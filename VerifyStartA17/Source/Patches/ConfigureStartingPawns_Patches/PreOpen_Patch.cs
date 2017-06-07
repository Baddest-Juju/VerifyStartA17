namespace VerifyStartA17.Patches.ConfigureStartingPawns_Patches {

    public static class PreOpen_Patch {

        public static void Prefix() {
            ConfigureStartingPawns_Controller.PreOpen();
        }
    }
}