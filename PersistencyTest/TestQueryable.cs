using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Persistency.Test
{
    internal class QueryProvider : IQueryProvider
    {
        private IQueryProvider _queryProvider;

        internal QueryProvider(IQueryProvider queryProvider)
        {
            _queryProvider = queryProvider;
        }

        public IQueryable CreateQuery(Expression expression) =>
            new TestQueryable(_queryProvider.CreateQuery(expression));

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) =>
            new TestQueryable<TElement>(_queryProvider.CreateQuery<TElement>(expression));

        public object Execute(Expression expression) =>
            _queryProvider.Execute(expression);

        public TResult Execute<TResult>(Expression expression) =>
            _queryProvider.Execute<TResult>(expression);
    }

    internal class TestQueryable : IQueryable
    {
        private readonly IQueryable _queryable;

        internal TestQueryable(IQueryable queryable)
        {
            _queryable = queryable;
        }

        public Type ElementType =>
            _queryable.ElementType;

        IEnumerator IEnumerable.GetEnumerator() =>
            _queryable.GetEnumerator();

        public Expression Expression =>
            _queryable.Expression;

        public IQueryProvider Provider =>
            new QueryProvider(_queryable.Provider);
    }

    internal class TestQueryable<T> : TestQueryable, IQueryable<T>, IAsyncEnumerableAccessor<T>
    {
        private readonly IQueryable<T> _genericQueryable;

        internal TestQueryable(IQueryable<T> queryable) : base(queryable)
        {
            _genericQueryable = queryable;
        }


        public IEnumerator<T> GetEnumerator() =>
            _genericQueryable.GetEnumerator();

        public IAsyncEnumerable<T> AsyncEnumerable =>
            new AsyncEnumerableClass(_genericQueryable.GetEnumerator());

        private class AsyncEnumerator : IAsyncEnumerator<T>
        {
            private IEnumerator<T> _enumerator;

            internal AsyncEnumerator(IEnumerator<T> enumerator)
            {
                _enumerator = enumerator;
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public async Task<bool> MoveNext(CancellationToken cancellationToken) =>
                _enumerator.MoveNext();

            public T Current => _enumerator.Current;
        }

        private class AsyncEnumerableClass : IAsyncEnumerable<T>
        {
            private readonly IEnumerator<T> _enumerator;

            internal AsyncEnumerableClass(IEnumerator<T> enumerator)
            {
                _enumerator = enumerator;
            }

            public IAsyncEnumerator<T> GetEnumerator() =>
                new AsyncEnumerator(_enumerator);
        }
    }
}