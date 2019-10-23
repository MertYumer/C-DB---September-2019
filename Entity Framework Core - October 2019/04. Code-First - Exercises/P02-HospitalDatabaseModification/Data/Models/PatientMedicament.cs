namespace P02_HospitalDatabaseModification.Data.Models
{
    using System.Collections.Generic;

    public class PatientMedicament
    {
        public PatientMedicament()
        {
            this.Prescriptions = new HashSet<PatientMedicament>();
        }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int MedicamentId { get; set; }
        public Medicament Medicament { get; set; }

        public ICollection<PatientMedicament> Prescriptions { get; set; }
    }
}
