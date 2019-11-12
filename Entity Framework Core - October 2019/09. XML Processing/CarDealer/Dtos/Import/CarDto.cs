namespace CarDealer.Dtos.Import
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlType("Car")]
    public class CarDto
    {
        [XmlElement("make")]
        public string Make { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("TraveledDistance")]
        public long TravelledDistance { get; set; }

        [XmlArray("parts")]
        public List<PartIdDto> Parts { get; set; }
    }
}
