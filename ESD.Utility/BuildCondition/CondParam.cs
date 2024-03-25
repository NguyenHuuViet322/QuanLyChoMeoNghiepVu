using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ESD.Utility.BuildCondition
{
    public static class BuildTest
    {
        public static string BuildWhere(this List<CondParam> lst)
        {
            var CondParams = new List<CondParam>();
            var where = "";
            var listWhere = new List<string>();
            int countClause = lst.Count;
            foreach (var item in lst)
            {
                var clause = string.IsNullOrEmpty(item.Clause) ? "and" : item.Clause;
                if (countClause - 1 == listWhere.Count)
                    clause = "";
                clause = clause.ToUpper();

                if (item.Sql.IsNotEmpty())
                {
                    listWhere.Add(item.Sql + " " + clause);
                }
                else
                {

                    switch (item.Operator)
                    {
                        case CondOperator.Equal:
                            listWhere.Add($" {item.FieldName} = '" + item.Value + "' " + clause + " ");
                            break;
                        case CondOperator.EqualDate:
                            listWhere.Add($" {item.FieldName} = " + item.Value + " " + clause + " ");
                            break;
                        case CondOperator.Lower:
                            listWhere.Add($" {item.FieldName} < {item.Value} {clause} ");
                            break;
                        case CondOperator.Greater:
                            listWhere.Add($" {item.FieldName} > {item.Value} {clause} ");
                            break;
                        case CondOperator.Like:
                            listWhere.Add($" lower({item.FieldName})  like ('%{(item.Value ?? string.Empty).ToString().ToLower()}%') {clause} ");
                            break;
                        case CondOperator.In:
                            listWhere.Add($" {item.FieldName} in ({item.Value}) {clause} ");//TODO
                            break;
                        case CondOperator.NotLike:
                            listWhere.Add($" {item.FieldName} Not like '%{item.Value}%' {clause} ");
                            break;
                        case CondOperator.NotIn:
                            listWhere.Add($" {item.FieldName} Not In ({ item.Value}) {clause} ");//TODO
                            break;
                        case CondOperator.NotEqual:
                            listWhere.Add($" {item.FieldName} != {item.Value} {clause} ");
                            break;
                        case CondOperator.LowerOrEqual:
                            listWhere.Add($" {item.FieldName} <= {item.Value} {clause} ");//TODO
                            break;
                        case CondOperator.GreaterOrEqual:
                            listWhere.Add($" {item.FieldName} <= {item.Value} {clause} ");//TODO
                            break;
                        case CondOperator.InRange://TODO
                            break;
                        case CondOperator.OutRange://TODO
                            break;
                        case CondOperator.SearchMeta://TODO
                            break;
                        case CondOperator.InRangeDate://TODO
                            break;
                        case CondOperator.Or:
                            break;
                        default:
                            break;
                    }
                }
            }
            return string.Join(" ", listWhere);
        }
    }
    public class CondParam
    {
        public string FieldName { get; set; }
        public CondOperator Operator { get; set; }
        public object Value { get; set; }
        public string Sql { get; set; }
        public object[] Arguments { get; set; }
        public bool NotUsed { get; set; }
        public string Clause { get; set; }

    }
    public enum CondOperator
    {
        [Description("Bằng")]
        Equal = 0,
        [Description("Nhỏ hơn")]
        Lower = 1,
        [Description("Lớn hơn")]
        Greater = 2,
        [Description("Chứa")]
        Like = 3,
        [Description("Thuộc")]
        In = 4,
        [Description("Không chứa")]
        NotLike = 5,
        [Description("Không thuộc")]
        NotIn = 6,
        [Description("Khác")]
        NotEqual = 7,
        [Description("Nhỏ hơn hoặc bằng")]
        LowerOrEqual = 8,
        [Description("Lớn hơn hoặc bằng")]
        GreaterOrEqual = 9,
        [Description("Trong khoảng")]
        InRange = 10,
        [Description("Ngoài khoảng")]
        OutRange = 11,
        [Description("Theo từ khóa")]
        SearchMeta = 12,
        [Description("Trong khoảng ngày tháng")]
        InRangeDate = 13,
        [Description("Hoặc")]
        Or = 14,
        [Description("Bằng ngày tháng")]
        EqualDate = 15
    }
}
