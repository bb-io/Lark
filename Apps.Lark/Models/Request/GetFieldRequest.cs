using Apps.Lark.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Lark.Models.Request
{
    public class GetFieldRequest
    {
        [Display("Field ID")]
        [DataSource(typeof(BaseTableFieldIdDataSourceHandler))]
        public string FieldId { get; set; }
    }

    public class GetTextFieldRequest
    {
        [Display("Field ID")]
        [DataSource(typeof(BaseTableTextFieldIdDataSourceHandler))]
        public string FieldId { get; set; }
    }

    public class GetPersonFieldRequest
    {
        [Display("Field ID")]
        [DataSource(typeof(BaseTablePersonFieldIdDataSourceHandler))]
        public string FieldId { get; set; }
    }
    public class GetDateFieldRequest
    {
        [Display("Field ID")]
        [DataSource(typeof(BaseTableDateFieldIdDataSourceHandler))]
        public string FieldId { get; set; }
    }

    public class GetMultipleFieldRequest
    {
        [Display("Field ID")]
        [DataSource(typeof(BaseTableMultipleFieldIdDataSourceHandler))]
        public string FieldId { get; set; }
    }

    public class GetNumberFieldRequest 
    {
        [Display("Field ID")]
        [DataSource(typeof(BaseTableNumberFieldIdDataSourceHandler))]
        public string FieldId { get; set; }
    }
}
