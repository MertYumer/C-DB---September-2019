namespace TeisterMask.DataProcessor.ExportDto
{
    using System.Xml.Serialization;

    [XmlType("Task")]
    public class ExportTaskXmlDto
    {
        public string Name { get; set; }

        public string Label { get; set; }
    }
}
