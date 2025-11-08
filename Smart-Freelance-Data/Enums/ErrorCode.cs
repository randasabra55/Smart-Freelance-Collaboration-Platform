namespace Smart_Freelance_Data.Enums
{
    public enum ErrorCode
    {
        None = 0,

        // Common Error Codes (1-999)
        ValidationError = 1,
        InvalidInput = 2,
        NotFound = 3,
        BadRequest = 4,
        InternalServerError = 5,
        ServiceUnavailable = 6,
        Timeout = 7,
        RateLimitExceeded = 8,
        NotAuthorized = 9,
        // Verification Codes
        VerificationCodeNotValid = 10,
        VerificationCodeExpired = 11,
        VerificationCodeAlreadyUsed = 12,
        TooManyVerificationAttempts = 13,
        RequiredFieldMissing = 14,

        // Files Operations
        FileRequired = 20,
        FileSizeExceedsLimit = 21,
        FileUploadFailed = 22,
        InvalidImageFile = 23,
        FileDeleteFailed = 24,
        FileNotFound = 25,
        InvalidFileType = 26,
        FileCorrupted = 27,

        // Database Operations
        DatabaseConnectionFailed = 30,
        DatabaseQueryFailed = 31,
        ConcurrencyError = 32,

        // Authentication Error Codes (1000-1999)
        InvalidCredentials = 1000,
        AuthenticationFailed = 1001,
        InvalidToken = 1002,
        TokenExpired = 1003,
        TokenRevoked = 1004,
        TokenCreationFailed = 1005,
        RefreshTokenInvalid = 1006,
        RefreshTokenExpired = 1007,
        MalformedToken = 1008,
        InvalidApiKey = 1009,

        // User Account Management
        UserNotFound = 1010,
        EmailNotConfirmed = 1011,
        AccountLocked = 1012,
        AccountDisabled = 1013,
        AccountSuspended = 1014,
        AccountNotActivated = 1015,
        PasswordExpired = 1016,
        IncorrectPassword = 1017,
        // PasswordAlreadyUsed=1018,
        // Registration & User Creation
        EmailAlreadyExists = 1020,
        InvalidEmail = 1028,
        UsernameAlreadyExists = 1021,
        RegistrationFailed = 1022,
        UnregisteredEmail = 1023,
        InvalidEmailFormat = 1024,
        WeakPassword = 1025,
        PasswordMismatch = 1026,
        PhoneNumberAlreadyExists = 1027,
        SameEmail = 1029,

        // Password Management
        PasswordResetFailed = 1030,
        PasswordResetTokenInvalid = 1031,
        PasswordResetTokenExpired = 1032,
        OldPasswordIncorrect = 1033,
        NewPasswordSameAsOld = 1034,
        PasswordHistoryViolation = 1035,

        // Two-Factor Authentication
        TwoFactorRequired = 1040,
        InvalidTwoFactorCode = 1041,
        TwoFactorCodeExpired = 1042,
        TwoFactorNotEnabled = 1043,
        TwoFactorAlreadyEnabled = 1044,
        TwoFactorSetupFailed = 1045,
        BackupCodeInvalid = 1046,
        NoBackupCodesRemaining = 1047,

        // External Authentication
        ExternalLoginFailed = 1050,
        ExternalLoginEmailRequired = 1051,
        UnsupportedExternalProvider = 1052,
        ExternalProviderError = 1053,
        ExternalAccountAlreadyLinked = 1054,
        ExternalAccountNotLinked = 1055,

        // Authorization Error Codes (2000-2999)
        AccessDenied = 2000,
        Forbidden = 2001,
        InsufficientPermissions = 2002,
        RoleNotFound = 2003,
        PermissionNotFound = 2004,
        ResourceOwnershipRequired = 2005,
        AdminAccessRequired = 2006,

        // Session Management
        SessionExpired = 2010,
        SessionNotFound = 2011,
        ConcurrentSessionLimitExceeded = 2012,
        SessionInvalidated = 2013,

        // Role & Permission Management
        RoleAssignmentFailed = 2020,
        RoleRemovalFailed = 2021,
        PermissionGrantFailed = 2022,
        PermissionRevokeFailed = 2023,
        CircularRoleReference = 2024,

        // API & Resource Access
        ApiQuotaExceeded = 2030,
        ResourceQuotaExceeded = 2031,
        IpAddressBlocked = 2032,
        GeolocationRestricted = 2033,

        // Data Update Failures
        FailedToUpdateData = 3000,
        FailedToCreateData = 3001,
        FailedToDeleteData = 3002,
        DataIntegrityViolation = 3003,
        OptimisticLockingFailure = 3004,
        DuplicatedName = 3005,







    }
}
