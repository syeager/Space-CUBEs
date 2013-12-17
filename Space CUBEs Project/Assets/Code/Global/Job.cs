// Steve Yeager
// 8.18.2013
// prime31studios

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;

/// <summary>
/// Wrapper for the coroutine class.
/// </summary>
public class Job
{
    #region Public Fields

    public bool paused { get; private set; }
    public bool running { get; private set; }

    #endregion

    #region Private Fields

    private IEnumerator coroutine;
    private bool killed;
    private float runtime;
    private Queue<Job> childJobs;

    #endregion

    #region Events

    public event Action<bool> JobCompleteEvent;

    #endregion


    #region Public Methods

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="coroutine">Coroutine to run.</param>
    /// <param name="start">Start right away?</param>
    public Job(IEnumerator coroutine, bool start = true)
    {
        this.coroutine = coroutine;
        if (start)
        {
            Start();
        }
    }


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="coroutine">Coroutine to run.</param>
    /// <param name="runtime">How long to run.</param>
    /// <param name="start">Start right away?</param>
    public Job(IEnumerator coroutine, float runtime, bool start = true)
    {
        this.coroutine = coroutine;
        this.runtime = runtime;
        if (start)
        {
            Start();
        }
    }


    /// <summary>
    /// Pause the job.
    /// </summary>
    public void Pause()
    {
        paused = true;
    }


    /// <summary>
    /// Unpause the job.
    /// </summary>
    public void UnPause()
    {
        paused = false;
    }


    /// <summary>
    /// Toggle pause.
    /// </summary>
    public void TogglePause()
    {
        paused = !paused;
    }


    /// <summary>
    /// Run the coroutine.
    /// </summary>
    public void Start()
    {
        running = true;
        JobManager.Main.StartCoroutine(Work());

        if (runtime > 0f)
        {
            End(runtime);
        }
    }


    /// <summary>
    /// Run the coroutine.
    /// </summary>
    /// <returns>Return the coroutine as it runs.</returns>
    public IEnumerator StartAsCoroutine()
    {
        running = true;
        if (runtime > 0f)
        {
            End(runtime);
        }
        yield return JobManager.Main.StartCoroutine(Work());
    }


    /// <summary>
    /// End Job by killing it.
    /// </summary>
    public void Kill()
    {
        if (childJobs != null)
        {
            while (childJobs.Count > 0)
            {
                childJobs.Dequeue().Kill();
            }
        }

        killed = true;
        running = false;
        paused = false;
    }


    /// <summary>
    /// End Job by killing it.
    /// </summary>
    /// <param name="delay">Time in seconds to delay before killing.</param>
    public void Kill(float delay)
    {
        delay *= 1000;
        new Timer(obj =>
        {
            lock (this)
            {
                Kill();
            }
        }, null, (int)delay, Timeout.Infinite);
    }


    /// <summary>
    /// End Job without killing it.
    /// </summary>
    public void End()
    {
        killed = false;
        running = false;
        paused = false;
    }


    /// <summary>
    /// Create a new child job.
    /// </summary>
    /// <param name="child">Child method.</param>
    /// <returns>New child job.</returns>
    public Job CreateChildJob(IEnumerator child, float runtime = 0f)
    {
        Job job = new Job(child, runtime, false);
        AddChildJob(job);
        return job;
    }


    /// <summary>
    /// Add an existing job as a child.
    /// </summary>
    /// <param name="child">Job to add.</param>
    public void AddChildJob(Job child)
    {
        if (childJobs == null)
        {
            childJobs = new Queue<Job>();
        }
        childJobs.Enqueue(child);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// End Job without killing it.
    /// </summary>
    /// <param name="delay">Delay in seconds before ending.</param>
    private void End(float delay)
    {
        delay *= 1000;
        new Timer(obj =>
        {
            lock (this)
            {
                End();
            }
        }, null, (int)delay, Timeout.Infinite);
    }


    /// <summary>
    /// Run, run children, or pause.
    /// </summary>
    private IEnumerator Work()
    {
        // return null in case of starting paused
        yield return null;

        while (running)
        {
            if (paused)
            {
                yield return null;
            }
            else
            {
                if (coroutine.MoveNext())
                {
                    yield return coroutine.Current;
                }
                else
                {
                    // run any child jobs
                    if (childJobs != null)
                    {
                        yield return JobManager.Main.StartCoroutine(RunChildJobs());
                    }

                    running = false;
                }
            }
        }

        // fire complete event
        if (JobCompleteEvent != null)
        {
            JobCompleteEvent(killed);
        }
    }


    /// <summary>
    /// Run all child jobs in order.
    /// </summary>
    private IEnumerator RunChildJobs()
    {
        if (childJobs != null && childJobs.Count > 0)
        {
            do
            {
                Job childJob = childJobs.Dequeue();
                yield return JobManager.Main.StartCoroutine(childJob.StartAsCoroutine());
            } while (childJobs.Count > 0);
        }
    }

    #endregion    
}