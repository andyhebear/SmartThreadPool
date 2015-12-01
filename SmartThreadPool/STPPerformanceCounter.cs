#define _SILVERLIGHT
#define _MONO
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
#if UNITY5
using System.Mono;
#endif

namespace Amib.Threading
{
    public interface ISTPPerformanceCountersReader
    {
        long InUseThreads { get; }
        long ActiveThreads { get; }
        long WorkItemsQueued { get; }
        long WorkItemsProcessed { get; }
    }

}

namespace Amib.Threading.Internal
{
    #if UNITY_WEBPLAYER

	public	abstract class CollectionBase : IList, ICollection, IEnumerable {

		// private instance properties
		private ArrayList list;
		
		// public instance properties
		public int Count { get { return InnerList.Count; } }
		
		// Public Instance Methods
		public IEnumerator GetEnumerator() { return InnerList.GetEnumerator(); }
		public void Clear() { 
			OnClear();
			InnerList.Clear(); 
			OnClearComplete();
		}
		public void RemoveAt (int index) {
			object objectToRemove;
			objectToRemove = InnerList[index];
			OnValidate(objectToRemove);
			OnRemove(index, objectToRemove);
			InnerList.RemoveAt(index);
			OnRemoveComplete(index, objectToRemove);
		}
		
		// Protected Instance Constructors
		protected CollectionBase()
		{ 
		}

		protected CollectionBase (int capacity)
		{
			list = new ArrayList (capacity);
		}

		//[ComVisible (false)]
		public int Capacity {
			get {
				if (list == null)
					list = new ArrayList ();
				
				return list.Capacity;
			}

			set {
				if (list == null)
					list = new ArrayList ();
							      
				list.Capacity = value;
			}
		}
		
		// Protected Instance Properties
		protected ArrayList InnerList {
			get {
				if (list == null)
					list = new ArrayList ();
				return list;
			} 
		}
		
		protected IList List {get { return this; } }
		
		// Protected Instance Methods
		protected virtual void OnClear() { }
		protected virtual void OnClearComplete() { }
		
		protected virtual void OnInsert(int index, object value) { }
		protected virtual void OnInsertComplete(int index, object value) { }

		protected virtual void OnRemove(int index, object value) { }
		protected virtual void OnRemoveComplete(int index, object value) { }

		protected virtual void OnSet(int index, object oldValue, object newValue) { }
		protected virtual void OnSetComplete(int index, object oldValue, object newValue) { }

		protected virtual void OnValidate(object value) {
			if (null == value) {
				throw new System.ArgumentNullException("CollectionBase.OnValidate: Invalid parameter value passed to method: null");
			}
		}
		
		// ICollection methods
		void ICollection.CopyTo(Array array, int index) {
			InnerList.CopyTo(array, index);
		}
		object ICollection.SyncRoot {
			get { return InnerList.SyncRoot; }
		}
		bool ICollection.IsSynchronized {
			get { return InnerList.IsSynchronized; }
		}

		// IList methods
		int IList.Add (object value) {
			int newPosition;
			OnValidate(value);
			newPosition = InnerList.Count;
			OnInsert(newPosition, value);
			InnerList.Add(value);
			try {
				OnInsertComplete(newPosition, value);
			} catch {
				InnerList.RemoveAt (newPosition);
				throw;
			}
			
			return newPosition;
		}
		
		bool IList.Contains (object value) {
			return InnerList.Contains(value);
		}

		int IList.IndexOf (object value) {
			return InnerList.IndexOf(value);
		}

		void IList.Insert (int index, object value) {
			OnValidate(value);
			OnInsert(index, value);
			InnerList.Insert(index, value);
			try {
				OnInsertComplete(index, value);
			} catch {
				InnerList.RemoveAt (index);
				throw;
			}
		}

		void IList.Remove (object value) {
			int removeIndex;
			OnValidate(value);
			removeIndex = InnerList.IndexOf(value);
			if (removeIndex == -1)
				throw new ArgumentException ("The element cannot be found.", "value");
			OnRemove(removeIndex, value);
			InnerList.Remove(value);
			OnRemoveComplete(removeIndex, value);
		}

		// IList properties
		bool IList.IsFixedSize { 
			get { return InnerList.IsFixedSize; }
		}

		bool IList.IsReadOnly { 
			get { return InnerList.IsReadOnly; }
		}

		object IList.this[int index] { 
			get { return InnerList[index]; }
			set { 
				if (index < 0 || index >= InnerList.Count)
					throw new ArgumentOutOfRangeException ("index");

				object oldValue;
				// make sure we have been given a valid value
				OnValidate(value);
				// save a reference to the object that is in the list now
				oldValue = InnerList[index];
				
				OnSet(index, oldValue, value);
				InnerList[index] = value;
				try {
					OnSetComplete(index, oldValue, value);
				} catch {
					InnerList[index] = oldValue;
					throw;
				}
			}
		}
	}
	[Serializable]
	public class CounterCreationDataCollection : CollectionBase {

		public CounterCreationDataCollection ()
		{
		}

		public CounterCreationDataCollection (
			CounterCreationData[] value)
		{
			AddRange (value);
		}

		public CounterCreationDataCollection (
			CounterCreationDataCollection value)
		{
			AddRange (value);
		}

		public CounterCreationData this [int index] {
			get {return (CounterCreationData) InnerList[index];}
			set {InnerList[index] = value;}
		}

		public int Add (CounterCreationData value)
		{
			return InnerList.Add (value);
		}

		public void AddRange (CounterCreationData[] value)
		{
			foreach (CounterCreationData v in value)
			{
				Add (v);
			}
		}

		public void AddRange (CounterCreationDataCollection value)
		{
			foreach (CounterCreationData v in value)
			{
				Add (v);
			}
		}

		public bool Contains (CounterCreationData value)
		{
			return InnerList.Contains (value);
		}

		public void CopyTo (CounterCreationData[] array, int index)
		{
			InnerList.CopyTo (array, index);
		}

		public int IndexOf (CounterCreationData value)
		{
			return InnerList.IndexOf (value);
		}

		public void Insert (int index, CounterCreationData value)
		{
			InnerList.Insert (index, value);
		}

		protected override void OnValidate (object value)
		{
			if (!(value is CounterCreationData))
				throw new NotSupportedException (Locale.GetText(
					"You can only insert " + 
					"CounterCreationData objects into " +
					"the collection"));
		}

		public virtual void Remove (CounterCreationData value)
		{
			InnerList.Remove (value);
		}
	}
    public class CounterCreationData 
	{

		// keep the same order of fields: this is used in metadata/mono-perfcounters.c
		private string help = String.Empty;
		private string name;
		private PerformanceCounterType type;

		public CounterCreationData ()
		{
		}

		public CounterCreationData (string counterName, 
			string counterHelp, 
			PerformanceCounterType counterType)
		{
			CounterName = counterName;
			CounterHelp = counterHelp;
			CounterType = counterType;
		}

		//[DefaultValue ("")]
		//[MonitoringDescription ("Description of this counter.")]
		public string CounterHelp {
			get {return help;}
			set {
				if (value == null)
					throw new ArgumentNullException ("value");
				help = value;
			}
		}

		//[DefaultValue ("")]
		//[MonitoringDescription ("Name of this counter.")]
		//[TypeConverter ("System.Diagnostics.Design.StringValueConverter, " + Consts.AssemblySystem_Design)]
		public string CounterName 
		{
			get {return name;}
			set {
				if (value == null)
					throw new ArgumentNullException ("value");
				if (value == "")
					throw new ArgumentException ("value");
				name = value;
			}
		}

		// may throw InvalidEnumArgumentException
		//[DefaultValue (typeof (PerformanceCounterType), "NumberOfItems32")]
		//[MonitoringDescription ("Type of this counter.")]
		public PerformanceCounterType CounterType {
			get {return type;}
			set {
				if (!Enum.IsDefined (typeof (PerformanceCounterType), value))
					throw new Exception ("typeof (PerformanceCounterType) not defined");
				type = value;
			}
		}
	}
#endif
    internal interface ISTPInstancePerformanceCounters : IDisposable
    {
        void Close();
        void SampleThreads(long activeThreads, long inUseThreads);
        void SampleWorkItems(long workItemsQueued, long workItemsProcessed);
        void SampleWorkItemsWaitTime(TimeSpan workItemWaitTime);
        void SampleWorkItemsProcessTime(TimeSpan workItemProcessTime);
    }
#if !(_WINDOWS_CE) && !(_SILVERLIGHT) && !(WINDOWS_PHONE)

    internal enum STPPerformanceCounterType
	{
		// Fields
		ActiveThreads				= 0,
		InUseThreads				= 1,
		OverheadThreads				= 2,
		OverheadThreadsPercent		= 3,
		OverheadThreadsPercentBase	= 4,

		WorkItems					= 5,
		WorkItemsInQueue			= 6,
		WorkItemsProcessed			= 7,

		WorkItemsQueuedPerSecond	= 8,
		WorkItemsProcessedPerSecond	= 9,

		AvgWorkItemWaitTime			= 10,
		AvgWorkItemWaitTimeBase		= 11,

		AvgWorkItemProcessTime		= 12,
		AvgWorkItemProcessTimeBase	= 13,

		WorkItemsGroups				= 14,

		LastCounter					= 14,
	}
 

	/// <summary>
	/// Summary description for STPPerformanceCounter.
	/// </summary>
	internal class STPPerformanceCounter
	{
		// Fields
		private readonly PerformanceCounterType _pcType;
		protected string _counterHelp;
		protected string _counterName;

		// Methods
		public STPPerformanceCounter(
			string counterName, 
			string counterHelp, 
			PerformanceCounterType pcType)
		{
			_counterName = counterName;
			_counterHelp = counterHelp;
			_pcType = pcType;
		}

		public void AddCounterToCollection(CounterCreationDataCollection counterData)
		{
			CounterCreationData counterCreationData = new CounterCreationData(
				_counterName, 
				_counterHelp, 
				_pcType);

			counterData.Add(counterCreationData);
		}
 
		// Properties
		public string Name
		{
			get
			{
				return _counterName;
			}
		}
	}

	internal class STPPerformanceCounters
	{
		// Fields
		internal STPPerformanceCounter[] _stpPerformanceCounters;
		private static readonly STPPerformanceCounters _instance;
		internal const string _stpCategoryHelp = "SmartThreadPool performance counters";
		internal const string _stpCategoryName = "SmartThreadPool";

		// Methods
		static STPPerformanceCounters()
		{
			_instance = new STPPerformanceCounters();
		}
 
		private STPPerformanceCounters()
		{
			STPPerformanceCounter[] stpPerformanceCounters = new STPPerformanceCounter[] 
				{ 
					new STPPerformanceCounter("Active threads", "The current number of available in the thread pool.", PerformanceCounterType.NumberOfItems32), 
					new STPPerformanceCounter("In use threads", "The current number of threads that execute a work item.", PerformanceCounterType.NumberOfItems32), 
					new STPPerformanceCounter("Overhead threads", "The current number of threads that are active, but are not in use.", PerformanceCounterType.NumberOfItems32), 
					new STPPerformanceCounter("% overhead threads", "The current number of threads that are active, but are not in use in percents.", PerformanceCounterType.RawFraction), 
					new STPPerformanceCounter("% overhead threads base", "The current number of threads that are active, but are not in use in percents.", PerformanceCounterType.RawBase), 

					new STPPerformanceCounter("Work Items", "The number of work items in the Smart Thread Pool. Both queued and processed.", PerformanceCounterType.NumberOfItems32), 
					new STPPerformanceCounter("Work Items in queue", "The current number of work items in the queue", PerformanceCounterType.NumberOfItems32), 
					new STPPerformanceCounter("Work Items processed", "The number of work items already processed", PerformanceCounterType.NumberOfItems32), 

					new STPPerformanceCounter("Work Items queued/sec", "The number of work items queued per second", PerformanceCounterType.RateOfCountsPerSecond32), 
					new STPPerformanceCounter("Work Items processed/sec", "The number of work items processed per second", PerformanceCounterType.RateOfCountsPerSecond32), 

					new STPPerformanceCounter("Avg. Work Item wait time/sec", "The average time a work item supends in the queue waiting for its turn to execute.", PerformanceCounterType.AverageCount64), 
					new STPPerformanceCounter("Avg. Work Item wait time base", "The average time a work item supends in the queue waiting for its turn to execute.", PerformanceCounterType.AverageBase), 

					new STPPerformanceCounter("Avg. Work Item process time/sec", "The average time it takes to process a work item.", PerformanceCounterType.AverageCount64), 
					new STPPerformanceCounter("Avg. Work Item process time base", "The average time it takes to process a work item.", PerformanceCounterType.AverageBase), 

					new STPPerformanceCounter("Work Items Groups", "The current number of work item groups associated with the Smart Thread Pool.", PerformanceCounterType.NumberOfItems32), 
				};

			_stpPerformanceCounters = stpPerformanceCounters;
			SetupCategory();
		}
 
		private void SetupCategory()
		{
			if (!PerformanceCounterCategory.Exists(_stpCategoryName))
			{
				CounterCreationDataCollection counters = new CounterCreationDataCollection();

				for (int i = 0; i < _stpPerformanceCounters.Length; i++)
				{
					_stpPerformanceCounters[i].AddCounterToCollection(counters);
				}

				PerformanceCounterCategory.Create(
					_stpCategoryName, 
					_stpCategoryHelp, 
                    PerformanceCounterCategoryType.MultiInstance,
					counters);
					
			}
		}
 
		// Properties
		public static STPPerformanceCounters Instance
		{
			get
			{
				return _instance;
			}
		}
 	}

	internal class STPInstancePerformanceCounter : IDisposable
	{
		// Fields
        private bool _isDisposed;
		private PerformanceCounter _pcs;

		// Methods
		protected STPInstancePerformanceCounter()
		{
            _isDisposed = false;
		}

		public STPInstancePerformanceCounter(
			string instance, 
			STPPerformanceCounterType spcType) : this()
		{
			STPPerformanceCounters counters = STPPerformanceCounters.Instance;
			_pcs = new PerformanceCounter(
				STPPerformanceCounters._stpCategoryName, 
				counters._stpPerformanceCounters[(int) spcType].Name, 
				instance, 
				false);
			_pcs.RawValue = _pcs.RawValue;
		}


		public void Close()
		{
			if (_pcs != null)
			{
				_pcs.RemoveInstance();
				_pcs.Close();
				_pcs = null;
			}
		}
 
		public void Dispose()
		{
            Dispose(true);
		}

        public virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Close();
                }
            }
            _isDisposed = true;
        }
 
		public virtual void Increment()
		{
			_pcs.Increment();
		}
 
		public virtual void IncrementBy(long val)
		{
			_pcs.IncrementBy(val);
		}

		public virtual void Set(long val)
		{
			_pcs.RawValue = val;
		}
	}

	internal class STPInstanceNullPerformanceCounter : STPInstancePerformanceCounter
	{
		// Methods
		public override void Increment() {}
		public override void IncrementBy(long value) {}
		public override void Set(long val) {}
	}



	internal class STPInstancePerformanceCounters : ISTPInstancePerformanceCounters
	{
        private bool _isDisposed;
		// Fields
		private STPInstancePerformanceCounter[] _pcs;
		private static readonly STPInstancePerformanceCounter _stpInstanceNullPerformanceCounter;

		// Methods
		static STPInstancePerformanceCounters()
		{
			_stpInstanceNullPerformanceCounter = new STPInstanceNullPerformanceCounter();
		}
 
		public STPInstancePerformanceCounters(string instance)
		{
            _isDisposed = false;
			_pcs = new STPInstancePerformanceCounter[(int)STPPerformanceCounterType.LastCounter];

            // Call the STPPerformanceCounters.Instance so the static constructor will
            // intialize the STPPerformanceCounters singleton.
			STPPerformanceCounters.Instance.GetHashCode();

			for (int i = 0; i < _pcs.Length; i++)
			{
				if (instance != null)
				{
					_pcs[i] = new STPInstancePerformanceCounter(
						instance, 
						(STPPerformanceCounterType) i);
				}
				else
				{
					_pcs[i] = _stpInstanceNullPerformanceCounter;
				}
			}
		}
 

		public void Close()
		{
			if (null != _pcs)
			{
				for (int i = 0; i < _pcs.Length; i++)
				{
                    if (null != _pcs[i])
                    {
                        _pcs[i].Dispose();
                    }
				}
				_pcs = null;
			}
		}

		public void Dispose()
		{
            Dispose(true);
		}

        public virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Close();
                }
            }
            _isDisposed = true;
        }
 
		private STPInstancePerformanceCounter GetCounter(STPPerformanceCounterType spcType)
		{
			return _pcs[(int) spcType];
		}

		public void SampleThreads(long activeThreads, long inUseThreads)
		{
			GetCounter(STPPerformanceCounterType.ActiveThreads).Set(activeThreads);
			GetCounter(STPPerformanceCounterType.InUseThreads).Set(inUseThreads);
			GetCounter(STPPerformanceCounterType.OverheadThreads).Set(activeThreads-inUseThreads);

			GetCounter(STPPerformanceCounterType.OverheadThreadsPercentBase).Set(activeThreads-inUseThreads);
			GetCounter(STPPerformanceCounterType.OverheadThreadsPercent).Set(inUseThreads);
		}

		public void SampleWorkItems(long workItemsQueued, long workItemsProcessed)
		{
			GetCounter(STPPerformanceCounterType.WorkItems).Set(workItemsQueued+workItemsProcessed);
			GetCounter(STPPerformanceCounterType.WorkItemsInQueue).Set(workItemsQueued);
			GetCounter(STPPerformanceCounterType.WorkItemsProcessed).Set(workItemsProcessed);

			GetCounter(STPPerformanceCounterType.WorkItemsQueuedPerSecond).Set(workItemsQueued);
			GetCounter(STPPerformanceCounterType.WorkItemsProcessedPerSecond).Set(workItemsProcessed);
		}

		public void SampleWorkItemsWaitTime(TimeSpan workItemWaitTime)
		{
			GetCounter(STPPerformanceCounterType.AvgWorkItemWaitTime).IncrementBy((long)workItemWaitTime.TotalMilliseconds);
			GetCounter(STPPerformanceCounterType.AvgWorkItemWaitTimeBase).Increment();
		}

		public void SampleWorkItemsProcessTime(TimeSpan workItemProcessTime)
		{
			GetCounter(STPPerformanceCounterType.AvgWorkItemProcessTime).IncrementBy((long)workItemProcessTime.TotalMilliseconds);
			GetCounter(STPPerformanceCounterType.AvgWorkItemProcessTimeBase).Increment();
		}
    }
#endif

    internal class NullSTPInstancePerformanceCounters : ISTPInstancePerformanceCounters, ISTPPerformanceCountersReader
	{
		private static readonly NullSTPInstancePerformanceCounters _instance = new NullSTPInstancePerformanceCounters();

		public static NullSTPInstancePerformanceCounters Instance
		{
			get { return _instance; }
		}

 		public void Close() {}
		public void Dispose() {}
 
		public void SampleThreads(long activeThreads, long inUseThreads) {}
		public void SampleWorkItems(long workItemsQueued, long workItemsProcessed) {}
		public void SampleWorkItemsWaitTime(TimeSpan workItemWaitTime) {}
		public void SampleWorkItemsProcessTime(TimeSpan workItemProcessTime) {}
        public long InUseThreads
        {
            get { return 0; }
        }

        public long ActiveThreads
        {
            get { return 0; }
        }

        public long WorkItemsQueued
        {
            get { return 0; }
        }

        public long WorkItemsProcessed
        {
            get { return 0; }
        }
	}

    internal class LocalSTPInstancePerformanceCounters : ISTPInstancePerformanceCounters, ISTPPerformanceCountersReader
    {
        public void Close() { }
        public void Dispose() { }

        private long _activeThreads;
        private long _inUseThreads;
        private long _workItemsQueued;
        private long _workItemsProcessed;

        public long InUseThreads
        {
            get { return _inUseThreads; }
        }

        public long ActiveThreads
        {
            get { return _activeThreads; }
        }

        public long WorkItemsQueued
        {
            get { return _workItemsQueued; }
        }

        public long WorkItemsProcessed
        {
            get { return _workItemsProcessed; }
        }

        public void SampleThreads(long activeThreads, long inUseThreads)
        {
            _activeThreads = activeThreads;
            _inUseThreads = inUseThreads;
        }

        public void SampleWorkItems(long workItemsQueued, long workItemsProcessed)
        {
            _workItemsQueued = workItemsQueued;
            _workItemsProcessed = workItemsProcessed;
        }

        public void SampleWorkItemsWaitTime(TimeSpan workItemWaitTime)
        {
            // Not supported
        }

        public void SampleWorkItemsProcessTime(TimeSpan workItemProcessTime)
        {
            // Not supported
        }
    }
}
