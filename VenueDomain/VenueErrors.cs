using SharedKernel;

namespace VenueDomain
{
    public static class VenueErrors
    {
        public static DomainException NullVenueName() =>
            DomainException.Build("NULL_VENUE_NAME", "عنوان باشگاه نباید خالی باشد.");

        public static DomainException InvalidVenueName() =>
            DomainException.Build("INVALID_VENUE_NAME", $"عنوان باشگاه نباید بیشتر از {Venue.NameMaxLength} کاراکتر باشد.");

        public static DomainException InvalidVenueStatusTransition(VenueStatus currentStatus, VenueStatus newStatus) =>
            DomainException.Build("INVALID_VENUE_STATUS_TRANSITION", $"تغییر وضعیت باشگاه از {currentStatus} به {newStatus} ممکن نیست.");

        public static DomainException DuplicateFacility(string facilityTitle) =>
            DomainException.Build("DUPLICATE_FACILITY", $"امکانات با عنوان {facilityTitle} قبلاً اضافه شده است.");

        public static DomainException FacilityNotFound(string facilityTitle) =>
            DomainException.Build("FACILITY_NOT_FOUND", $"امکانات با عنوان {facilityTitle} یافت نشد.");

        public static DomainException InvalidFacilityTitle() =>
            DomainException.Build("INVALID_FACILITY_TITLE", "عنوان امکانات نباید خالی باشد.");

        public static DomainException InvalidAdminUserId() =>
            DomainException.Build("INVALID_ADMIN_USER_ID", "شناسه‌ی کاربری ادمین نباید خالی باشد.");

        public static DomainException DuplicateAdmin(string userId) =>
            DomainException.Build("DUPLICATE_ADMIN", $"ادمین با شناسه کاربری '{userId}' قبلاً اضافه شده است.");

        public static DomainException AdminNotFound(string userId) =>
            DomainException.Build("ADMIN_NOT_FOUND", $"ادمین با شناسه کاربری '{userId}' یافت نشد.");

        public static DomainException AdminRemoveLimit() =>
            DomainException.Build("ADMIN_LIMIT", "باشگاه باید حداقل یک ادمین داشته باشد.");

        public static DomainException InvalidAddress() =>
            DomainException.Build("INVALID_ADDRESS", "آدرس وارد شده معتبر نمی باشد.");

        public static DomainException InvalidPostalCode() =>
            DomainException.Build("INVALID_POSTAL_CODE", "کد پستی باید ۱۰ رقم باشد.");

        public static DomainException InvalidLocation() =>
            DomainException.Build("INVALID_LOCATION", "موقعیت جغرافیایی الزامی است.");

        public static DomainException InvalidLatitude() =>
            DomainException.Build("INVALID_LATITUDE", "عرض جغرافیایی وارد شده معتبر نمی باشد.");

        public static DomainException InvalidLongitude() =>
            DomainException.Build("INVALID_LONGITUDE", "طول جغرافیایی وارد شده معتبر نمی باشد.");
    }
}
