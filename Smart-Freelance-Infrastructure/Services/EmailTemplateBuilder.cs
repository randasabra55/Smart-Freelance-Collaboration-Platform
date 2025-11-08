namespace Smart_Freelance_Infrastructure.Services
{
    public class EmailTemplateBuilder
    {
        public static string BuildTokenEmail(string token, string userName = "User")
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Verification OTP</title>
            </head>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
                <div style='background-color: #f8f9fa; padding: 30px; border-radius: 10px;'>
                    <h2 style='color: #2c3e50; margin-bottom: 20px;'>Hello {userName}!</h2>
                    <p>Your verification OTP is:</p>
                    <div style='background-color: #e3f2fd; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                        <code style='font-size: 18px; font-weight: bold; color: #1565c0;'>{token}</code>
                    </div>
                    <p style='color: #666; font-size: 14px;'>This OTP will expire in 15 minutes.</p>
                    <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                    <p style='color: #888; font-size: 12px;'>This is an automated message, please do not reply.</p>
                </div>
            </body>
            </html>";
        }

        public static string BuildResetPasswordEmail(string token, string userName = "User")
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Reset Password OTP</title>
            </head>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
                <div style='background-color: #f8f9fa; padding: 30px; border-radius: 10px;'>
                    <h2 style='color: #d32f2f; margin-bottom: 20px;'>Password Reset Request</h2>
                    <p>Hello {userName},</p>
                    <p>You have requested to reset your password. Please use the following OTP to proceed:</p>
                    <div style='background-color: #ffebee; padding: 15px; border-radius: 5px; margin: 20px 0; border-left: 4px solid #d32f2f;'>
                        <code style='font-size: 18px; font-weight: bold; color: #d32f2f;'>{token}</code>
                    </div>
                    <p style='color: #666; font-size: 14px;'>This OTP will expire in 15 minutes.</p>
                    <p style='color: #d32f2f; font-size: 14px; font-weight: bold;'>If you didn't request this password reset, please ignore this email and your password will remain unchanged.</p>
                    <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'>
                    <p style='color: #888; font-size: 12px;'>This is an automated message, please do not reply.</p>
                </div>
            </body>
            </html>";
        }
    }
}
