using System.Windows.Controls;

namespace Hospital.App.Views;

public partial class PatientBanner : UserControl
{
    public PatientBanner()
    {
        InitializeComponent();
    }

    public void SetPatient(string name, string gender, string? birthDate, string? phone, string? idCard, string patientNo)
    {
        PatientName.Text = name;
        PatientInfo.Text = $"{gender} | {(birthDate ?? "--")} | {phone ?? "--"}";
        PatientIdLabel.Text = $"病历号: {patientNo} | 身份证: {idCard ?? "--"}";
        AvatarText.Text = name.Length > 0 ? name[..1] : "?";
    }

    public void Clear()
    {
        PatientName.Text = "--";
        PatientInfo.Text = "--";
        PatientIdLabel.Text = "--";
        AvatarText.Text = "?";
    }
}
