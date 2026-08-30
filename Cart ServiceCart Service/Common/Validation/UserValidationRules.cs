namespace Cart_ServiceCart_Service.Common.Validation;

public static class UserValidationRules
{
    public const int FullNameMaxLength = 150;
    public const int EmailMaxLength = 256;
    public const int PhoneMaxLength = 20;
    public const int DocumentUrlMaxLength = 1024;
    public const int DocumentTypeMaxLength = 100;
    public const string PhonePattern = @"^\+?[1-9]\d{7,19}$";
}