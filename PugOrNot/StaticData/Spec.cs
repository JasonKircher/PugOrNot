namespace DefaultNamespace;

public class Spec
{
    private bool bl = false;
    private bool br = false;

    public bool hasBL()
    {
        return this.bl;
    }

    public bool hasBR()
    {
        return this.br;
    }
}

public class HeroSpec : Spec
{
    private bool bl = true;
}

public class BLSpec : Spec
{
    private bool br = true;
}