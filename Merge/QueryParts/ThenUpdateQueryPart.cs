﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CompleteSQL.Extension;
using CompleteSQL.Merge.QueryPartBuilders;

namespace CompleteSQL.Merge
{
    
    public enum ParamAliasSet
    {
        TargetSource,
        OnlySource,
        OnlyTarget,
        Empty
    }
    public sealed class ThenUpdateQueryPart : QueryPartDecorator
    {
        private readonly LambdaExpression m_columnExpr;

        private readonly ParamAliasSet m_paramAliasSet;

        #region ctors

        internal ThenUpdateQueryPart()
        {
           
        }

        internal ThenUpdateQueryPart(LambdaExpression columnExpr, ParamAliasSet paramAliasSet)
        {
            m_columnExpr = columnExpr;
            m_paramAliasSet = paramAliasSet;
        }

        #endregion


        internal override string GetQueryPart()
        {


            if (m_columnExpr == null)
            {
                string[] mergePredicateNames = null;
               switch(mergePredicateBody.NodeType)
               {
                   case ExpressionType.MemberAccess:
                       mergePredicateNames = new string[1];
                       mergePredicateNames[0] = ((MemberExpression)mergePredicateBody).Member.Name;
                       break;

                   case ExpressionType.New:

                       mergePredicateNames = ((NewExpression)mergePredicateBody).Arguments
                                                                            .Select(expr => ((MemberExpression)expr).Member.Name)
                                                                            .ToArray();
                       break;

                   default: throw new ArgumentException();
                      
               }

               string[] updateColumnNames = tableSchema.Columns
                                                .Select(col => col.Name)
                                                .Except(mergePredicateNames)
                                                .ToArray();

               string updateOperators = string.Join("," + Environment.NewLine,
                              updateColumnNames.Select(item=>
                                  string.Concat("\t\ttgt.", item, " = src.", item)));

               string update = string.Concat("\tThen Update Set ", Environment.NewLine, updateOperators);

               return string.Concat(base.GetQueryPart(), Environment.NewLine, update);

            }
            else
            {


                switch (m_columnExpr.Body.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        break;
                    case ExpressionType.New:



                        NewExpression newBody = (NewExpression)m_columnExpr.Body;


                        NewValueBuilder builder =
                        NewValueBuilder.CreateBuilder(newBody);

                        switch (m_paramAliasSet)
                        {
                            case ParamAliasSet.OnlyTarget:
                                builder.SetTgtParameter(m_columnExpr.Parameters[0]);
                                break;
                            case ParamAliasSet.OnlySource:
                                builder.SetSrcParameter(m_columnExpr.Parameters[0]);
                                break;
                            case ParamAliasSet.TargetSource:
                                builder.SetTgtParameter(m_columnExpr.Parameters[0]);
                                builder.SetSrcParameter(m_columnExpr.Parameters[1]);
                                break;
                        }

                        var newValues = builder.GetNewValues();


                        var tgtColumns = newBody.GetTargetColumnNames();



                        string updateOperators = string.Join("," + Environment.NewLine,
                            tgtColumns.Select((item, index) =>
                                string.Concat("\t\ttgt.", item, " = ", newValues[index])));

                        string update = string.Concat("\tThen Update Set ", Environment.NewLine, updateOperators);

                        return string.Concat(base.GetQueryPart(), Environment.NewLine, update);


                    default: throw new ArgumentException();
                }

            }

            throw new NotImplementedException();
        }
    }
}
