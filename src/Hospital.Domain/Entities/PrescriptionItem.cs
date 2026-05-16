namespace Hospital.Domain.Entities;

/// <summary>处方明细实体</summary>
public class PrescriptionItem : Entity
{
    // EF Core
    private PrescriptionItem() { }

    public PrescriptionItem(long prescriptionId, string drugName, string spec, string form,
        string freq, string dosage, int duration, int qty, string? note)
    {
        PrescriptionId = prescriptionId;
        DrugName = drugName;
        Spec = spec;
        Form = form;
        Freq = freq;
        Dosage = dosage;
        Duration = duration;
        Qty = qty;
        Note = note ?? string.Empty;
    }

    public long PrescriptionId { get; private set; }
    public string DrugName { get; private set; } = string.Empty;
    public string Spec { get; private set; } = string.Empty;
    public string Form { get; private set; } = string.Empty;
    public string Freq { get; private set; } = string.Empty;
    public string Dosage { get; private set; } = string.Empty;
    public int Duration { get; private set; }
    public int Qty { get; private set; }
    public string Note { get; private set; } = string.Empty;
}
