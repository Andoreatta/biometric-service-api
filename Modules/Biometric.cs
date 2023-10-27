using BiometricService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NITGEN.SDK.NBioBSP;
using System.Text.Json.Nodes;

public class Biometric
{
    private readonly APIService APIServiceInstance;

    public Biometric(APIService apiService)
    {
        APIServiceInstance = apiService;
    }
    public IActionResult CaptureHash()
    {
        APIServiceInstance._NBioAPI.OpenDevice(NBioAPI.Type.DEVICE_ID.AUTO);
        uint ret = APIServiceInstance._NBioAPI.Capture(NBioAPI.Type.FIR_PURPOSE.ENROLL, out NBioAPI.Type.HFIR hCapturedFIR, NBioAPI.Type.TIMEOUT.DEFAULT, null, null);
        APIServiceInstance._NBioAPI.CloseDevice(NBioAPI.Type.DEVICE_ID.AUTO);
        if (ret != NBioAPI.Error.NONE) return new BadRequestObjectResult($"Error on Capture: {ret}");

        APIServiceInstance._NBioAPI.GetTextFIRFromHandle(hCapturedFIR, out NBioAPI.Type.FIR_TEXTENCODE textFIR, true);

        return new OkObjectResult(textFIR.TextFIR);
    }

    public IActionResult IdentifyOneOnOne(string fingerprintHash)
    {
        var secondFir = new NBioAPI.Type.FIR_TEXTENCODE { TextFIR = fingerprintHash };
        APIServiceInstance._NBioAPI.OpenDevice(NBioAPI.Type.DEVICE_ID.AUTO);
        uint ret = APIServiceInstance._NBioAPI.Verify(secondFir, out bool matched, null);
        APIServiceInstance._NBioAPI.CloseDevice(NBioAPI.Type.DEVICE_ID.AUTO);
        if (ret != NBioAPI.Error.NONE) return new BadRequestObjectResult($"Error on Verify: {ret}");
        
        if (matched)
            return new OkObjectResult(true);
        else
            return new OkObjectResult(false);
    }

    public IActionResult Identification()
    {
        APIServiceInstance._NBioAPI.OpenDevice(NBioAPI.Type.DEVICE_ID.AUTO);
        uint ret = APIServiceInstance._NBioAPI.Capture(NBioAPI.Type.FIR_PURPOSE.VERIFY, out NBioAPI.Type.HFIR hCapturedFIR, NBioAPI.Type.TIMEOUT.DEFAULT, null, null);
        APIServiceInstance._NBioAPI.CloseDevice(NBioAPI.Type.DEVICE_ID.AUTO);
        if (ret != NBioAPI.Error.NONE) return new BadRequestObjectResult($"Error on Capture: {ret}");

        APIServiceInstance._IndexSearch.IdentifyData(hCapturedFIR, NBioAPI.Type.FIR_SECURITY_LEVEL.NORMAL, out NBioAPI.IndexSearch.FP_INFO fpInfo, null);
        if (fpInfo.ID != 0)
            return new OkObjectResult(fpInfo.ID);
        else
            return new OkObjectResult("No match found");
    }

    public IActionResult LoadToMemory(Finger[] fingers)
    {
        if (fingers.Length == 0) return new BadRequestObjectResult("No templates to load");

        var textFir = new NBioAPI.Type.FIR_TEXTENCODE();
        foreach (Finger fingerObject in fingers)
        {
            textFir.TextFIR = fingerObject.Template;
            APIServiceInstance._IndexSearch.AddFIR(textFir, fingerObject.Id, out _);
        }
        return new OkObjectResult("Templates loaded to memory");
    }

    public IActionResult DeleteAllFromMemory()
    {
        APIServiceInstance._IndexSearch.ClearDB();
        return new OkObjectResult("All templates deleted from memory");
    }
}