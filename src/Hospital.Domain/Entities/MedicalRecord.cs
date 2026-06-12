using Hospital.Domain.Enums;

namespace Hospital.Domain.Entities;

/// <summary>病历记录实体</summary>
public class MedicalRecord : Entity
{
    // EF Core
    private MedicalRecord() { }

    public MedicalRecord(long encounterId, string chiefComplaint)
    {
        EncounterId = encounterId;
        ChiefComplaint = chiefComplaint;
        PresentIllness = string.Empty;
        PastHistory = string.Empty;
        PhysicalExam = string.Empty;
        Status = RecordStatus.草稿;
        Version = 1;
    }

    public long EncounterId { get; private set; }
    public string ChiefComplaint { get; private set; } = string.Empty;
    public string PresentIllness { get; private set; } = string.Empty;
    public string PastHistory { get; private set; } = string.Empty;
    public string PhysicalExam { get; private set; } = string.Empty;
    public RecordStatus Status { get; private set; }
    public int Version { get; private set; }

    /// <summary>保存草稿</summary>
    public void SaveDraft(string chiefComplaint, string presentIllness, string pastHistory, string physicalExam)
    {
        ChiefComplaint = chiefComplaint;
        PresentIllness = presentIllness;
        PastHistory = pastHistory;
        PhysicalExam = physicalExam;
        Status = RecordStatus.草稿;
    }

    /// <summary>提交终稿</summary>
    public void Submit()
    {
        Status = RecordStatus.终稿;
    }

    /// <summary>更新已终稿的病历（版本递增）</summary>
    public void Update(string chiefComplaint, string presentIllness, string pastHistory, string physicalExam)
    {
        ChiefComplaint = chiefComplaint;
        PresentIllness = presentIllness;
        PastHistory = pastHistory;
        PhysicalExam = physicalExam;
        Status = RecordStatus.已修改;
        Version++;
    }
}
