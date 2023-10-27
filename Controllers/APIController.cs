using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace BiometricService.Controllers
{
    [ApiController]
    [Route("apiservice/")]
    public class APIController : ControllerBase
    {
        private readonly Biometric _biometric;

        public APIController(Biometric biometric)
        {
            _biometric = biometric;
        }

        [HttpGet("capture-hash")]
        public IActionResult Capture()
        {
            return _biometric.CaptureHash();
        }

        [HttpPost("match-one-on-one")]
        public IActionResult MatchOneOnOne([FromBody] string fingerprintHash)
        {
            return _biometric.IdentifyOneOnOne(fingerprintHash);
        }

        [HttpGet("identification")]
        public IActionResult Identification()
        {
            return _biometric.Identification();
        }

        [HttpPost("load-to-memory")]
        public IActionResult LoadToMemory([FromBody] Finger[] fingers)
        {
            return _biometric.LoadToMemory(fingers);
        }

        [HttpGet("delete-all-from-memory")]
        public IActionResult DeleteAllFromMemory()
        {
            return _biometric.DeleteAllFromMemory();
        }
    }
}