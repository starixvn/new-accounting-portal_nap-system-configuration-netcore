namespace NapAdminCore.Criteria.OTP;

using Fanex.Data.Repository;

public class SetupOTPUpdateCommand : NonQueryCommand
{
    public int CustId { get; set; }

    public string SecretKey { get; set; }

    public string System { get; set; }

    public bool IgnoreOTP { get; set; }

    public int Modifier { get; set; }

    public string LoginIP { get; set; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "NAP_Admin_OTPUpd";

    public override bool IsValid() => true;
}
