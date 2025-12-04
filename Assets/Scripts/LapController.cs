using UnityEngine;
using UnityEngine.Events;

public class LapController : MonoBehaviour
{
    public int totalLaps = 2;
    public int currentLap { get; private set; } = 0;
    
    private bool passedCheckpoint = false;
    
    // public UnityEvent<int> onLapChanged;
    // public UnityEvent onRaceWon;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FinishLine"))
        {
            HandleFinishLine();
        }

        if (other.CompareTag("Checkpoint"))
        {
            HandleCheckpoint();
        }
    }

    private void HandleFinishLine()
    {
        // Caso A: Empezar la carrera (Vuelta 1)
        if (currentLap == 0)
        {
            StartLap(1);
        }
        // Caso B: Completar una vuelta
        else
        {
            if (passedCheckpoint)
            {
                CompleteLap();
            }
            else
            {
                // Hizo trampa
                Debug.LogWarning("¡Te saltaste el checkpoint!");
            }
        }
    }

    private void HandleCheckpoint()
    {
        if (currentLap > 0)
        {
            Debug.Log("¡Checkpoint!");
            passedCheckpoint = true;
        }
    }

    private void StartLap(int lapNumber)
    {
        currentLap = lapNumber;
        passedCheckpoint = false; // Resetea la "llave" para la nueva vuelta
        Debug.Log("¡CARRERA INICIADA! Vuelta " + currentLap);
        // onLapChanged?.Invoke(currentLap);
    }
    
    private void CompleteLap()
    {
        currentLap++;
        
        if (currentLap > totalLaps)
        {
            WinRace();
        }
        else
        {
            // Empezar la siguiente vuelta
            StartLap(currentLap);
        }
    }
    
    private void WinRace()
    {   //si quermos podemos modificar para cambiarlo
        Debug.Log("¡¡¡GANASTE LA CARRERA!!!");
        Time.timeScale = 0; 
    }
}