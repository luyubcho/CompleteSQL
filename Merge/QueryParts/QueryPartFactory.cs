﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CompleteSQL.Merge.QueryPartsFactory
{
    internal static class QueryPartFactory
    {
        internal static WhenMatchedQueryPart CreateWhenMatchedQueryPart(this QueryPartComponent queryComponent)
        {
            var whenMatchedQueryPart = new WhenMatchedQueryPart();
            whenMatchedQueryPart.QueryPartComponent = queryComponent;

            return whenMatchedQueryPart;
        }

        internal static AndTargetQueryPart CreateWhenMatchedAndQueryPart<TSource>(this QueryPartComponent queryComponent, Expression<Func<TSource, bool>> predicate)
        {
            var whenMatchedQueryPart = new WhenMatchedQueryPart();
            whenMatchedQueryPart.QueryPartComponent = queryComponent;

            var andQueryPart = new AndTargetQueryPart(predicate);
            andQueryPart.QueryPartComponent = whenMatchedQueryPart;

            return andQueryPart;
        }

       
        internal static WhenNotMatchedQueryPart CreateWhenNotMatchedByTargetQueryPart(this QueryPartComponent queryComponent)
        {
            var whenNotMatched = new WhenNotMatchedQueryPart(true);
            whenNotMatched.QueryPartComponent = queryComponent;

            return whenNotMatched;
        }

        internal static WhenNotMatchedQueryPart CreateWhenNotMatchedBySourceQueryPart(this QueryPartComponent queryComponent)
        {
            var whenNotMatchedBySource = new WhenNotMatchedQueryPart(false);
            whenNotMatchedBySource.QueryPartComponent = queryComponent;

            return whenNotMatchedBySource;
        }

        internal static AndSourceQueryPart CreateWhenNotMatchedByTargetAndQueryPart<TSource>(this QueryPartComponent queryComponent, Expression<Func<TSource, bool>> predicate)
        {
            var whenNotMatchedByTarget = new WhenNotMatchedQueryPart(true);
            whenNotMatchedByTarget.QueryPartComponent = queryComponent;

            var andQueryPart = new AndSourceQueryPart(predicate);
            andQueryPart.QueryPartComponent = whenNotMatchedByTarget;

            return andQueryPart;
        }

        internal static AndTargetQueryPart CreateWhenNotMatcheBySourceAndQueryPart<TSource>(this QueryPartComponent queryComponent, Expression<Func<TSource, bool>> predicate)
        {
            var whenNotMatchedBySource = new WhenNotMatchedQueryPart(false);
            whenNotMatchedBySource.QueryPartComponent = queryComponent;

            var andQueryPart = new AndTargetQueryPart(predicate);
            andQueryPart.QueryPartComponent = whenNotMatchedBySource;

            return andQueryPart;
        }

        internal static ThenUpdateQueryPart CreateWNMThenUpdateQueryPart<TSource, TUpdate>(this QueryPartComponent queryComponent, Expression<Func<TSource, TUpdate>> updatingColumns)
        {
            var thenUpdateQueryPart = new ThenUpdateQueryPart(updatingColumns);
            thenUpdateQueryPart.QueryPartComponent = queryComponent;

            return thenUpdateQueryPart;
        }

        internal static ThenUpdateQueryPart CreateWMThenUpdateQueryPart<TSource, TUpdate>(this QueryPartComponent queryComponent, Expression<Func<TSource, TUpdate>> updatingColumns)
        {
            var thenUpdateQueryPart = new ThenUpdateQueryPart(updatingColumns);
            thenUpdateQueryPart.QueryPartComponent = queryComponent;

            return thenUpdateQueryPart;
        }

        internal static ThenDeleteQueryPart CreateTDeleteQueryPart(this QueryPartComponent queryComponent)
        {
            var thenDeleteQueryPart = new ThenDeleteQueryPart();
            thenDeleteQueryPart.QueryPartComponent = queryComponent;
            return thenDeleteQueryPart;
        }
    }
}
