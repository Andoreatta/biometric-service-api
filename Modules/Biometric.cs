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
        try
        {
            APIServiceInstance._NBioAPI.OpenDevice(NBioAPI.Type.DEVICE_ID.AUTO);
            uint ret = APIServiceInstance._NBioAPI.Capture(NBioAPI.Type.FIR_PURPOSE.ENROLL, out NBioAPI.Type.HFIR hCapturedFIR, NBioAPI.Type.TIMEOUT.DEFAULT, null, null);
            APIServiceInstance._NBioAPI.CloseDevice(NBioAPI.Type.DEVICE_ID.AUTO);
            if (ret != NBioAPI.Error.NONE) throw new Exception("Error on Capture");

            APIServiceInstance._NBioAPI.GetTextFIRFromHandle(hCapturedFIR, out NBioAPI.Type.FIR_TEXTENCODE textFIR, true);

            return textFIR.TextFIR;

        }
        catch (Exception ex)
        {
            throw new Exception($"Error capturing fingerprint: {ex.Message}");
        }
    }

    public bool IdentifyOneOnOne(string fingerprintHash)
    {
        var secondFir = new NBioAPI.Type.FIR_TEXTENCODE { TextFIR = fingerprintHash };
        try
        {
            APIServiceInstance._NBioAPI.OpenDevice(NBioAPI.Type.DEVICE_ID.AUTO);
            APIServiceInstance._NBioAPI.Verify(secondFir, out bool matched, null);
            APIServiceInstance._NBioAPI.CloseDevice(NBioAPI.Type.DEVICE_ID.AUTO);
            if (matched)
                return true;
            else
                return false;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error matching templates: {ex.Message}");
        }
    }

    public uint Identification()
    {
        try
        {
            APIServiceInstance._NBioAPI.OpenDevice(NBioAPI.Type.DEVICE_ID.AUTO);
            uint ret = APIServiceInstance._NBioAPI.Capture(NBioAPI.Type.FIR_PURPOSE.VERIFY, out NBioAPI.Type.HFIR hCapturedFIR, NBioAPI.Type.TIMEOUT.DEFAULT, null, null);
            APIServiceInstance._NBioAPI.CloseDevice(NBioAPI.Type.DEVICE_ID.AUTO);
            if (ret != NBioAPI.Error.NONE) throw new Exception("Error on Capture");

            APIServiceInstance._IndexSearch.IdentifyData(hCapturedFIR, NBioAPI.Type.FIR_SECURITY_LEVEL.NORMAL, out NBioAPI.IndexSearch.FP_INFO fpInfo, null);
            if (fpInfo.ID != 0)
                return fpInfo.ID;
            else
                return 0;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error identifying fingerprint: {ex.Message}");
        }
    }

    public bool LoadToMemory(Finger[] fingers)
    {
        var textFir = new NBioAPI.Type.FIR_TEXTENCODE();
        try
        {
            foreach (Finger fingerObject in fingers)
            {
                textFir.TextFIR = fingerObject.Template;
                APIServiceInstance._IndexSearch.AddFIR(textFir, fingerObject.Id, out _);
            }
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error loading templates to memory: {ex.Message}");
        }
    }

    public string DeleteAllFromMemory()
    {
        try
        {
            APIServiceInstance._IndexSearch.ClearDB();
            return "All templates deleted from memory";
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting templates from memory: {ex.Message}");
        }
    }
}