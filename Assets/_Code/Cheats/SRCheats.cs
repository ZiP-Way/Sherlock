using System.ComponentModel;
using Data;
using Profile;
using SignalsFramework;

// ReSharper disable UnusedMember.Global -> Reflection

// ReSharper disable once CheckNamespace -> Has to be in the same namespace
public partial class SROptions
{
    //private const string LevelsControl = "LevelsControl";
    private const string AddCurrency = "+ Currency";
    private const string RemoveCurrency = "- Currency";

    //[Category(LevelsControl)]
    //public int CurrentLevel
    //{
    //    get => PlayerProfile.CurrentLevel;
    //    set
    //    {
    //        if (value >= 0)
    //        {
    //PlayerProfile.CurrentLevel = value;
    //        }
    //    }
    //}

    //[Category(LevelsControl)]
    //public void LoadSelectedLevel()
    //{
    //    Hub.LevelFailed.Fire();
    //}

    [Category(AddCurrency)]
    public void AddSoftCurrency()
    {
        PlayerProfile.SoftCurrency += 1;
        Hub.ForceRefreshSoftCurrency.Fire();
    }

    [Category(AddCurrency)]
    public void AddTenSoftCurrency()
    {
        PlayerProfile.SoftCurrency += 10;
        Hub.ForceRefreshSoftCurrency.Fire();
    }

    [Category(AddCurrency)]
    public void AddHundredSoftCurrency()
    {
        PlayerProfile.SoftCurrency += 100;
        Hub.ForceRefreshSoftCurrency.Fire();
    }

    [Category(AddCurrency)]
    public void AddThousandSoftCurrency()
    {
        PlayerProfile.SoftCurrency += 1000;
        Hub.ForceRefreshSoftCurrency.Fire();
    }


    [Category(RemoveCurrency)]
    public void RemoveSoftCurrency()
    {
        PlayerProfile.SoftCurrency -= 1;
        Hub.ForceRefreshSoftCurrency.Fire();
    }

    [Category(RemoveCurrency)]
    public void RemoveTenSoftCurrency()
    {
        PlayerProfile.SoftCurrency -= 10;
        Hub.ForceRefreshSoftCurrency.Fire();
    }

    [Category(RemoveCurrency)]
    public void RemoveHundredSoftCurrency()
    {
        PlayerProfile.SoftCurrency -= 100;
        Hub.ForceRefreshSoftCurrency.Fire();
    }

    [Category(RemoveCurrency)]
    public void RemoveThousandSoftCurrency()
    {
        PlayerProfile.SoftCurrency -= 1000;
        Hub.ForceRefreshSoftCurrency.Fire();
    }
}
