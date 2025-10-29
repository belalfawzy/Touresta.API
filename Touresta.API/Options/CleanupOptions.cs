namespace Touresta.API.Options
{
    public class CleanupOptions

    {
        // يشتغل كل قد اي يا حب 
        public int IntervalMinutes { get; set; } = 5;

        // السماح للحذف كل قد اي 
        public int GraceHours { get; set; } = 6;

        // الحد الاقصي لعمر الحساب الغير مغعل قبل ما يمسحلك ام ال otp
        public int HardDeleteHours { get; set; } = 48;
    }
}
