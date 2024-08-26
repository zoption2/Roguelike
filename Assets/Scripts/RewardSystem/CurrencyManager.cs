using SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public interface ICurrencyManager
{
    public int GetCurrencyAmount(CurrencyType type);
    public void AddSomeCurrency(CurrencyType type, int count);
    public void SubstractSomeCurrency(CurrencyType type, int count);
}
public class CurrencyManager : ICurrencyManager
{
    private IDataService _dataService;
    private Dictionary<CurrencyType, int> _currencies = new Dictionary<CurrencyType, int>();
    public CurrencyManager(IDataService dataService)
    {
        _dataService = dataService;
        Init();
    }

    private void Init()
    {
        int count;
        List<CurrencyType> types = Enum.GetValues(typeof(CurrencyType)).Cast<CurrencyType>().ToList();
        foreach (CurrencyType type in types)
        {
            count = _dataService.PlayerData.GetAmountOfCurrency(type);
            _currencies.Add(type, count);
        }
    }

    public int GetCurrencyAmount(CurrencyType type)
    {
        if (_currencies.ContainsKey(type))
        {
            return _currencies[type];
        }
        else
            return 0;
    }

    public void AddSomeCurrency(CurrencyType type, int count)
    {
        if (_currencies.ContainsKey(type))
        {
            Debug.Log("existing currency");
            _currencies[type] += count;
        }
        else
        {
            Debug.Log("new currency");
            _currencies.Add(type, count);
        }

        Debug.Log("amount of " + type + ": " + _currencies[type]);
        _dataService.PlayerData.SetAmountOfCurrency(type, _currencies[type]);
    }

    public void SubstractSomeCurrency(CurrencyType type, int count)
    {
        if (_currencies.ContainsKey(type))
        {
            _currencies[type] -= count;
        }
        else
            _currencies.Add(type, 0);

        _dataService.PlayerData.SetAmountOfCurrency(type, _currencies[type]);
    }

}
