using Data.Data;

namespace Data.Services;

public static class Localization
{
    public static string Localize(Position position) => position switch
    {
        Position.Goalkeeper => "Golman",
        Position.Defender   => "Branič",
        Position.Midfielder => "Vezni",
        Position.Forward    => "Napadač",
        _                   => position.ToString()
    };

    public static string LocalizeShort(Position position) => position switch
    {
        Position.Goalkeeper => "GK",
        Position.Defender   => "CB",
        Position.Midfielder => "MF",
        Position.Forward    => "FW",
        _                   => position.ToString()
    };

    public static string Localize(SurfaceType surface) => surface switch
    {
        SurfaceType.NaturalGrass    => "Prirodna trava",
        SurfaceType.ArtificialGrass => "Umjetna trava",
        SurfaceType.Concrete        => "Beton",
        SurfaceType.Indoor          => "Zatvoreni prostor",
        _                           => surface.ToString()
    };

    public static string Localize(FieldStatus status) => status switch
    {
        FieldStatus.Verified          => "Verificirano",
        FieldStatus.PendingApproval   => "Čeka odobrenje",
        FieldStatus.UnderMaintenance  => "U održavanju",
        _                             => status.ToString()
    };

    public static string LocalizeOutdoor(bool isOutdoor) => isOutdoor ? "Na otvorenom" : "Zatvoreno";
}
