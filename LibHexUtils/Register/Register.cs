namespace LibHexUtils.Register
{
    public class Register
    {
        private int reg = 0;

        public Register(int initState = 0)
        {
            this.reg = initState;
        }

        public bool IsPresent(in int flag)
        {
            return (reg & flag) != 0;
        }

        public void AddFlag(in int flag)
        {
            reg |= flag;
        }

        public void RemFlag(in int flag)
        {
            reg &= (~flag);
        }

        #region Functions
        /// <summary>
        /// Check if is flag present in register (AND)
        /// </summary>
        /// <param name="register">Input register</param>
        /// <param name="flag">Checking flag</param>
        /// <returns>True if is present, otherwise false</returns>
        public static bool IsPresent(in int register, in int flag)
        {
            return (register & flag) != 0;
        }

        /// <summary>
        /// Add flag to register (OR)
        /// </summary>
        /// <param name="register">Changed register</param>
        /// <param name="flag">Flag to add</param>
        public static void AddFlag(ref int register, in int flag)
        {
            register |= flag;
        }

        /// <summary>
        /// Remove flag from register (AND-XOR)
        /// </summary>
        /// <param name="register">Changed register</param>
        /// <param name="flag">Flag to remove</param>
        public static void RemFlag(ref int register, in int flag)
        {
            register &= (~flag);
        }

        #endregion
    }
}
