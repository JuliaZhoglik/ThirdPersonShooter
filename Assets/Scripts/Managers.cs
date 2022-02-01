using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(InventoryManager))]

public class Managers : MonoBehaviour
{
    public static PlayerManager Player { get; private set; } // Managers.PlayerManager
    public static InventoryManager Inventory { get; private set; } // Managers.Inventory

    private List<IGameManager> _startSequence;

    void Awake() // вызывается до Start
    {
        Player = GetComponent<PlayerManager>();
        Inventory = GetComponent<InventoryManager>();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(Player);
        _startSequence.Add(Inventory);

        StartCoroutine(StartupManagers()); // асинхронно загружаем стартовую последовательность
    }

    private IEnumerator StartupManagers()
    {
        foreach (IGameManager manager in _startSequence)
        {
            manager.Startup();
        }

        yield return null;

        int numModules = _startSequence.Count;
        int numReady = 0;

        // продолжаем цикл, пока не начнут работать все диспетчеры
        while (numReady < numModules)
        {
            int lastReady = numReady;
            numReady = 0;

            foreach (IGameManager manager in _startSequence)
            {
                if (manager.status == ManagerStatus.Started)
                {
                    Debug.Log("manager Started " + manager.ToString());
                    numReady++;
                }
            }

            if (numReady > lastReady) Debug.Log("Progress: " + numReady + "/" + numModules);
            yield return null; // остановка на один кадр перед следующей проверкой
        }

        Debug.Log("All managers started up");

    }

}
