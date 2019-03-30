using System;

using Lumen.Lang;

public static class Main {
    public static void Import(Scope scope, String s) {
        scope.Set("Visual", Lumen.Lang.Libraries.Visual.Visual.Instance);
    }
}
