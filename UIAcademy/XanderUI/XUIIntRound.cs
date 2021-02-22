namespace LibUIAcademy.XanderUI
{
    public static class XUIIntRound
    {
        // Methods
        public static int IntRound(this int number, Round rounding)
        {
            int num = 0;
            if (rounding == Round.Ten)
            {
                num = ((number % 10) > 5) ? ((number + 10) - (number % 10)) : (number - (number % 10));
            }
            if (rounding == Round.Hundred)
            {
                num = ((number % 100) > 50) ? ((number + 100) - (number % 100)) : (number - (number % 100));
            }
            if (rounding == Round.Thousand)
            {
                num = ((number % 0x3e8) > 500) ? ((number + 0x3e8) - (number % 0x3e8)) : (number - (number % 0x3e8));
            }
            if (rounding == Round.TenThousand)
            {
                num = ((number % 0x2710) > 0x1388) ? ((number + 0x2710) - (number % 0x2710)) : (number - (number % 0x2710));
            }
            if (rounding == Round.HundredThousand)
            {
                num = ((number % 0x1_86a0) > 0xc350) ? ((number + 0x1_86a0) - (number % 0x1_86a0)) : (number - (number % 0x1_86a0));
            }
            if (rounding == Round.Million)
            {
                num = ((number % 0xf_4240) > 0x7_a120) ? ((number + 0xf_4240) - (number % 0xf_4240)) : (number - (number % 0xf_4240));
            }
            if (rounding == Round.TenMillion)
            {
                num = ((number % 0x98_9680) > 0x4c_4b40) ? ((number + 0x98_9680) - (number % 0x98_9680)) : (number - (number % 0x98_9680));
            }
            if (rounding == Round.HundredMillion)
            {
                num = ((number % 0x5f5_e100) > 0x2fa_f080) ? ((number + 0x5f5_e100) - (number % 0x5f5_e100)) : (number - (number % 0x5f5_e100));
            }
            if (rounding == Round.Billion)
            {
                num = ((number % 0x3b9a_ca00) > 0x1dcd_6500) ? ((number + 0x3b9a_ca00) - (number % 0x3b9a_ca00)) : (number - (number % 0x3b9a_ca00));
            }
            return num;
        }

        public static int IntRoundDown(this int number, Round rounding)
        {
            int num = 0;
            if (rounding == Round.Ten)
            {
                num = number - (number % 10);
            }
            if (rounding == Round.Hundred)
            {
                num = number - (number % 100);
            }
            if (rounding == Round.Thousand)
            {
                num = number - (number % 0x3e8);
            }
            if (rounding == Round.TenThousand)
            {
                num = number - (number % 0x2710);
            }
            if (rounding == Round.HundredThousand)
            {
                num = number - (number % 0x1_86a0);
            }
            if (rounding == Round.Million)
            {
                num = number - (number % 0xf_4240);
            }
            if (rounding == Round.TenMillion)
            {
                num = number - (number % 0x98_9680);
            }
            if (rounding == Round.HundredMillion)
            {
                num = number - (number % 0x5f5_e100);
            }
            if (rounding == Round.Billion)
            {
                num = number - (number % 0x3b9a_ca00);
            }
            return num;
        }

        public static int IntRoundUp(this int number, Round rounding)
        {
            int num = 0;
            if (rounding == Round.Ten)
            {
                num = (number + 10) - (number % 10);
            }
            if (rounding == Round.Hundred)
            {
                num = (number + 100) - (number % 100);
            }
            if (rounding == Round.Thousand)
            {
                num = (number + 0x3e8) - (number % 0x3e8);
            }
            if (rounding == Round.TenThousand)
            {
                num = (number + 0x2710) - (number % 0x2710);
            }
            if (rounding == Round.HundredThousand)
            {
                num = (number + 0x1_86a0) - (number % 0x1_86a0);
            }
            if (rounding == Round.Million)
            {
                num = (number + 0xf_4240) - (number % 0xf_4240);
            }
            if (rounding == Round.TenMillion)
            {
                num = (number + 0x98_9680) - (number % 0x98_9680);
            }
            if (rounding == Round.HundredMillion)
            {
                num = (number + 0x5f5_e100) - (number % 0x5f5_e100);
            }
            if (rounding == Round.Billion)
            {
                num = (number + 0x3b9a_ca00) - (number % 0x3b9a_ca00);
            }
            return num;
        }

        // Nested Types
        public enum Round
        {
            Ten,
            Hundred,
            Thousand,
            TenThousand,
            HundredThousand,
            Million,
            TenMillion,
            HundredMillion,
            Billion
        }
    }
}
