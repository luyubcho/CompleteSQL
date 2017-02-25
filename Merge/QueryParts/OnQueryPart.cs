﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CompleteSQL.Merge
{
    public sealed class OnQueryPart : MergeQueryPartDecorator
    {
        LambdaExpression m_predicate;
        internal OnQueryPart(Expression expr)
        {
            m_predicate = (LambdaExpression)expr;
        }
        internal override string GetQueryPart()
        {
            string predicate = string.Empty;

            switch(m_predicate.Body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    MemberExpression memberBody = (MemberExpression)m_predicate.Body;
                    predicate = string.Format("tgt.{0} = src.{0}", memberBody.Member.Name);
                    break;
                case ExpressionType.New:
                    NewExpression newBody = (NewExpression)m_predicate.Body;

                    predicate = string.Join(Environment.NewLine + "\tAnd ",
                        newBody.Members.Select(m => string.Format("tgt.{0} = src.{0}", m.Name)));

                  
                    break;
                default:
                    throw new ArgumentException();
            }

            string query = string.Concat(base.GetQueryPart(), "\tOn ", predicate);

            return query;
        }
    }
}
