using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Functions
{
    public static class SqlFunctionTranslations
    {
        /// <summary>
        /// LINQ Translation for SQL Function Search Score
        /// CASE DATALENGTH(@value) == 0 THEN 0 ELSE (100 * (DATALENGTH(@value) - DATALENGTH(REPLACE(@value, @keyword, ''))) / DATALENGTH(@value))
        /// </summary>
        /// <returns>Function containing LINQ translation</returns>
        public static Func<IReadOnlyCollection<SqlExpression>, SqlExpression> GetSearchScoreTranslation()
        {
            return args =>
                    new CaseExpression(
                        operand: new SqlFunctionExpression("DATALENGTH",
                                        new SqlExpression[] { args.Skip(1).First() },
                                        nullable: true,
                                        argumentsPropagateNullability: new[] { true },
                                        args.First().Type,
                                        args.First().TypeMapping),
                        whenClauses: new List<CaseWhenClause> { new CaseWhenClause(new SqlConstantExpression(Expression.Constant(0), new IntTypeMapping("int", DbType.Int32)), new SqlConstantExpression(Expression.Constant(0), new IntTypeMapping("int", DbType.Int32))) },
                        elseResult: new CaseExpression(
                                operand: new SqlConstantExpression(Expression.Constant(true), new BoolTypeMapping("bit", DbType.Boolean)),
                                whenClauses: new List<CaseWhenClause>
                                {
                                    new CaseWhenClause(new SqlBinaryExpression(ExpressionType.GreaterThanOrEqual,
                                                new SqlFunctionExpression("DATALENGTH",
                                                        new SqlExpression[] { args.Skip(1).First() },true, new[] { true }, args.First().Type, args.First().TypeMapping),
                                                new SqlFunctionExpression("DATALENGTH",
                                                        new SqlExpression[] { args.First() },true, new[] { true }, args.First().Type, args.First().TypeMapping),
                                                args.First().Type,
                                                args.First().TypeMapping),
                                    result: new SqlBinaryExpression(
                                                ExpressionType.Multiply,
                                                left: new SqlConstantExpression(
                                                    Expression.Constant(100),
                                                    new IntTypeMapping("int", DbType.Int32)),
                                                right: new SqlBinaryExpression(
                                                    ExpressionType.Divide,
                                                    left: new SqlBinaryExpression(
                                                        ExpressionType.Subtract,
                                                        left: new SqlFunctionExpression(
                                                                "DATALENGTH",
                                                                new SqlExpression[] { args.Skip(1).First() },
                                                                nullable: true,
                                                                argumentsPropagateNullability: new[] { true },
                                                                args.First().Type,
                                                                args.First().TypeMapping),
                                                        right: new SqlFunctionExpression(
                                                                "DATALENGTH",
                                                                new SqlExpression[]
                                                                {
                                                                    new SqlFunctionExpression(
                                                                        "REPLACE",
                                                                        new SqlExpression[] { args.Skip(1).First(), args.First(), new SqlConstantExpression(Expression.Constant(""), new StringTypeMapping("varchar",DbType.String))},
                                                                        nullable: true,
                                                                        argumentsPropagateNullability: new[] { true, true, true },
                                                                        args.First().Type,
                                                                        args.First().TypeMapping)
                                                                },
                                                                nullable: true,
                                                                argumentsPropagateNullability: new[] { true },
                                                                args.First().Type,
                                                                args.First().TypeMapping),

                                                        type: args.First().Type,
                                                        typeMapping: args.First().TypeMapping),
                                                    right: new SqlFunctionExpression(
                                                                "DATALENGTH",
                                                                new SqlExpression[]
                                                                { args.Skip(1).First() },
                                                                nullable: true,
                                                                argumentsPropagateNullability: new[] { true },
                                                                args.First().Type,
                                                                args.First().TypeMapping),
                                                        args.First().Type,
                                                        args.First().TypeMapping),
                                                args.First().Type,
                                                args.First().TypeMapping))
                                },
                                elseResult: new SqlBinaryExpression(
                        ExpressionType.Multiply,
                        left: new SqlConstantExpression(
                            Expression.Constant(100),
                            new IntTypeMapping("int", DbType.Int32)),
                        right: new SqlBinaryExpression(
                            ExpressionType.Divide,
                            left: new SqlBinaryExpression(
                                ExpressionType.Subtract,
                                left: new SqlFunctionExpression(
                                        "DATALENGTH",
                                        new SqlExpression[] { args.First() },
                                        nullable: true,
                                        argumentsPropagateNullability: new[] { true },
                                        args.First().Type,
                                        args.First().TypeMapping),
                                right: new SqlFunctionExpression(
                                        "DATALENGTH",
                                        new SqlExpression[]
                                        {
                                            new SqlFunctionExpression(
                                                "REPLACE",
                                                new SqlExpression[] { args.First(), args.Skip(1).First(), new SqlConstantExpression(Expression.Constant(""), new StringTypeMapping("varchar",DbType.String))},
                                                nullable: true,
                                                argumentsPropagateNullability: new[] { true, true, true },
                                                args.First().Type,
                                                args.First().TypeMapping)
                                        },
                                        nullable: true,
                                        argumentsPropagateNullability: new[] { true },
                                        args.First().Type,
                                        args.First().TypeMapping),

                                type: args.First().Type,
                                typeMapping: args.First().TypeMapping),
                            right: new SqlFunctionExpression(
                                        "DATALENGTH",
                                        new SqlExpression[]
                                        { args.First() },
                                        nullable: true,
                                        argumentsPropagateNullability: new[] { true },
                                        args.First().Type,
                                        args.First().TypeMapping),
                                args.First().Type,
                                args.First().TypeMapping),
                        args.First().Type,
                        args.First().TypeMapping)
                            )
                        );
        }
    }
}
