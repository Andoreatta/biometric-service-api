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
        if (ret != NBioAPI.Error.NONE) return new BadRequestObjectResult(
            new JsonObject
            {
                ["message"] = $"Error on Capture: {ret}",
                ["success"] = false
            }
        );

        APIServiceInstance._NBioAPI.GetTextFIRFromHandle(hCapturedFIR, out NBioAPI.Type.FIR_TEXTENCODE textFIR, true);

        return new OkObjectResult(
            new JsonObject
            {
                ["template"] = textFIR.TextFIR,
                ["success"] = true,
            }
        );
    }

    public IActionResult IdentifyOneOnOne(JsonObject template)
    {
        var secondFir = new NBioAPI.Type.FIR_TEXTENCODE { TextFIR = template["template"]?.ToString() };

        APIServiceInstance._NBioAPI.OpenDevice(NBioAPI.Type.DEVICE_ID.AUTO);
        uint ret = APIServiceInstance._NBioAPI.Verify(secondFir, out bool matched, null);
        APIServiceInstance._NBioAPI.CloseDevice(NBioAPI.Type.DEVICE_ID.AUTO);
        if (ret != NBioAPI.Error.NONE) return new BadRequestObjectResult(
            new JsonObject
            {
                ["message"] = ret == NBioAPI.Error.CAPTURE_TIMEOUT ? "Timeout" : $"Error on Verify: {ret}",
                ["success"] = false
            }
        );

        return new OkObjectResult(
            new JsonObject
            {
                ["message"] = matched ? "Fingerprint matches" : "Fingerprint doesnt match",
                ["success"] = matched }
            );
    }

    public IActionResult Identification()
    {
        APIServiceInstance._NBioAPI.OpenDevice(NBioAPI.Type.DEVICE_ID.AUTO);
        uint ret = APIServiceInstance._NBioAPI.Capture(NBioAPI.Type.FIR_PURPOSE.VERIFY, out NBioAPI.Type.HFIR hCapturedFIR, NBioAPI.Type.TIMEOUT.DEFAULT, null, null);
        APIServiceInstance._NBioAPI.CloseDevice(NBioAPI.Type.DEVICE_ID.AUTO);
        if (ret != NBioAPI.Error.NONE) return new BadRequestObjectResult(
            new JsonObject
            {
                ["message"] = $"Error on Capture: {ret}",
                ["success"] = false
            }
        );

        NBioAPI.IndexSearch.CALLBACK_INFO_0 cbInfo = new();
        APIServiceInstance._IndexSearch.IdentifyData(hCapturedFIR, NBioAPI.Type.FIR_SECURITY_LEVEL.NORMAL, out NBioAPI.IndexSearch.FP_INFO fpInfo, cbInfo);

        return new OkObjectResult(
            new JsonObject
            {
                ["message"] = fpInfo.ID != 0 ? "Fingerprint match found" : "Fingerprint match not found",
                ["id"] = fpInfo.ID,
                ["success"] = fpInfo.ID != 0
            }
        );

    }

    public IActionResult LoadToMemory(JsonArray fingers)
    {
        if (fingers.Count == 0)
        {
            return new BadRequestObjectResult(
                new JsonObject
                {
                    ["message"] = "No templates to load",
                    ["success"] = false
                }
            );
        }

        uint ret;
        var textFir = new NBioAPI.Type.FIR_TEXTENCODE();
        foreach (JsonObject fingerObject in fingers)
        {
            textFir.TextFIR = fingerObject["template"].ToString();
            ret = APIServiceInstance._IndexSearch.AddFIR(textFir, (uint)fingerObject["id"], out _);
            if (ret != NBioAPI.Error.NONE) return new BadRequestObjectResult(
                new JsonObject
                {
                    ["message"] = $"Error on AddFIR: {ret}",
                    ["success"] = false
                }
            );
        }

        return new OkObjectResult(
            new JsonObject
            {
                ["message"] = "Templates loaded to memory",
                ["success"] = true
            }
        );
    }

    public IActionResult DeleteAllFromMemory()
    {
        APIServiceInstance._IndexSearch.ClearDB();
        return new OkObjectResult(
            new JsonObject
            {
                ["message"] = "All templates deleted from memory",
                ["success"] = true
            }
        );
    }
}