using BiometricService;
using NITGEN.SDK.NBioBSP;

public class Biometric
{
    private readonly APIService APIServiceInstance;

    public Biometric(APIService apiService)
    {
        APIServiceInstance = apiService;
    }

    public string CaptureHash()
    {
        APIServiceInstance._NBioAPI.OpenDevice(NBioAPI.Type.DEVICE_ID.AUTO);
        uint ret = APIServiceInstance._NBioAPI.Capture(NBioAPI.Type.FIR_PURPOSE.VERIFY, out NBioAPI.Type.HFIR hCapturedFIR, NBioAPI.Type.TIMEOUT.DEFAULT, null, null);
        APIServiceInstance._NBioAPI.CloseDevice(NBioAPI.Type.DEVICE_ID.AUTO);
        if (ret != NBioAPI.Error.NONE) return "Error on Capture";

        APIServiceInstance._NBioAPI.GetTextFIRFromHandle(hCapturedFIR, out NBioAPI.Type.FIR_TEXTENCODE textFIR, true);

        return textFIR.TextFIR;
    }

    //public string MatchOneOnOne(string fingerprint_hash)
    //{

    //}

    public string Identification(){
        APIServiceInstance._NBioAPI.OpenDevice(NBioAPI.Type.DEVICE_ID.AUTO);
        uint ret = APIServiceInstance._NBioAPI.Capture(NBioAPI.Type.FIR_PURPOSE.VERIFY, out NBioAPI.Type.HFIR hCapturedFIR, NBioAPI.Type.TIMEOUT.DEFAULT, null, null);
        APIServiceInstance._NBioAPI.CloseDevice(NBioAPI.Type.DEVICE_ID.AUTO);
        if (ret != NBioAPI.Error.NONE) return "Error on Capture";

        APIServiceInstance._IndexSearch.IdentifyData(hCapturedFIR, NBioAPI.Type.FIR_SECURITY_LEVEL.NORMAL, out NBioAPI.IndexSearch.FP_INFO fpInfo, null);
        if (fpInfo.ID != 0)
            return fpInfo.ID.ToString();
        else
            return "Identification failed, no match found";
    }

    public string LoadToMemory(Finger[] fingers)
    {
        var textFir = new NBioAPI.Type.FIR_TEXTENCODE();
        try
        {
            foreach (Finger fingerObject in fingers)
            {
                textFir.TextFIR = fingerObject.Template;
                APIServiceInstance._IndexSearch.AddFIR(textFir, fingerObject.Id, out _);
            }
            return "Templates loaded successfully";
        }
        catch (Exception ex)
        {
            return $"Error loading templates: {ex.Message}";
        }
    }

    public string DeleteAllFromMemory()
    {
        APIServiceInstance._IndexSearch.ClearDB();
        return "All templates deleted successfully";
    }
}