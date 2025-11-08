
using Microsoft.AspNetCore.Mvc;
using Smart_Freelance_API.Bases;
using Smart_Freelance_Core.Features.Payment.Commands;
using Smart_Freelance_Core.Features.Payment.Queries;

namespace Smart_Freelance_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : AppControllerBase
    {


        ///Webhook endpoint for Paymob callbacks

        [HttpPost("PaymobWebhook")]
        public async Task<IResult> PaymobWebhook([FromBody] PaymobWebhookCommand command)
        {
            var result = await Mediator.Send(command);
            if (!result.IsSuccess)
                return Results.BadRequest(result.Error);
            return Results.Ok(result.Data);
        }


        //Get payment iframe HTML for a transaction

        [HttpGet("GetPaymentIframe")]
        public async Task<IResult> GetPaymentIframe([FromQuery] GetPaymentIframeQuery query)
        {
            var result = await Mediator.Send(query);
            if (!result.IsSuccess)
                return Results.BadRequest(result.Error);
            return Results.Ok(result.Data);
        }
    }
}