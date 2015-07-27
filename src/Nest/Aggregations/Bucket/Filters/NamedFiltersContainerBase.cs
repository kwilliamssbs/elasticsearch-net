﻿
using System;
using System.Collections.Generic;
using System.Linq;
using Nest.Resolvers.Converters;
using Newtonsoft.Json;

namespace Nest
{
	[JsonConverter(typeof(CompositeJsonConverter<ReadAsTypeConverter<NamedFiltersContainer>, DictionaryKeysAreNotFieldNamesJsonConverter>))]
	public interface INamedFiltersContainer
	{
	}

	[JsonConverter(typeof(CompositeJsonConverter<ReadAsTypeConverter<NamedFiltersContainer>, DictionaryKeysAreNotFieldNamesJsonConverter>))]
	public abstract class NamedFiltersContainerBase : ProxyDictionary<string, IQueryContainer>, INamedFiltersContainer
	{
		protected NamedFiltersContainerBase () : base() { }
		protected NamedFiltersContainerBase(IDictionary<string, IQueryContainer> container) : base(container) { }

		public static implicit operator NamedFiltersContainerBase(Dictionary<string, IQueryContainer> container) =>
			new NamedFiltersContainer(container);

		public static implicit operator NamedFiltersContainerBase(Dictionary<string, QueryContainer> container) =>
			new NamedFiltersContainer(container);
	}

	public class NamedFiltersContainer: NamedFiltersContainerBase
	{
		public NamedFiltersContainer() : base() { }
		public NamedFiltersContainer(IDictionary<string, IQueryContainer> container) : base(container) { }
		public NamedFiltersContainer(Dictionary<string, QueryContainer> container)
			: base(container.Select(kv => kv).ToDictionary(kv => kv.Key, kv => (IQueryContainer)kv.Value))
		{ }

		public void Add(string name, IQueryContainer filter) => _backingDictionary.Add(name, filter);
		public void Add(string name, QueryContainer filter) => _backingDictionary.Add(name, filter);
	}

	public class NamedFiltersContainerDescriptor<T>: NamedFiltersContainerBase
		where T : class
	{
		public NamedFiltersContainerDescriptor() : base() { }
		protected NamedFiltersContainerDescriptor(IDictionary<string, IQueryContainer> container) : base(container) { }

		public NamedFiltersContainerDescriptor<T> Filter(string name, IQueryContainer filter)
		{
			 _backingDictionary.Add(name, filter);
			return this;
		}

		public NamedFiltersContainerDescriptor<T> Filter(string name, Func<QueryContainerDescriptor<T>, IQueryContainer> selector)
		{
			var filter = selector?.Invoke(new QueryContainerDescriptor<T>());
			if (filter != null) _backingDictionary.Add(name, filter);
			return this;
		}
		public NamedFiltersContainerDescriptor<T> Filter<TOther>(string name, Func<QueryContainerDescriptor<TOther>, IQueryContainer> selector)
			where TOther : class
		{
			var filter = selector?.Invoke(new QueryContainerDescriptor<TOther>());
			if (filter != null) _backingDictionary.Add(name, filter);
			return this;
		}
	}

}