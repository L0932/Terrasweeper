using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FyberPlugin;

public class FyberManager : MonoBehaviour {

    //string placementId;
    Ad rewardedVideoAd, interstitialAd, ofwAd;
    private string appID = "92614";


    public void RequestRewardedVideo()
    {
        
        RewardedVideoRequester.Create()
        //optional method chaining
        //.AddParameter("key", "value")
        //.AddParameters(dictionary)
        .WithPlacementId("rewardedVideo")
        // changing the GUI notification behaviour when the user finishes viewing the video
        .NotifyUserOnCompletion(true)
        // you can add a virtual currency requester to a video requester instead of requesting it separately
        //.WithVirtualCurrencyRequester(virtualCurrencyRequester)
        // you don't need to add a callback if you are using delegates
        //.WithCallback(requestCallback)
        // requesting the video
        .Request();
    }


	// Use this for initialization
	void Start () {
		Settings settings = Fyber.With(appID)
			// optional chaining methods
			//.WithUserId(userId)
			//.WithParameters(dictionary)
			//.WithSecurityToken(securityToken)
			//.WithManualPrecaching()
			.Start();
	}

    void OnEnable()
    {
        // Ad availability
        FyberCallback.AdAvailable += OnAdAvailable;
        FyberCallback.AdNotAvailable += OnAdNotAvailable;

        // Generic request error
        FyberCallback.RequestFail += OnRequestFail;

        FyberCallback.AdStarted += OnAdStarted;
        FyberCallback.AdFinished += OnAdFinished;

        // Exception in the native code
        FyberCallback.NativeError += OnNativeExceptionReceivedFromSDK;
    }
	
    void OnDisable()
    {
        FyberCallback.AdAvailable -= OnAdAvailable;
        FyberCallback.AdNotAvailable -= OnAdNotAvailable;

        FyberCallback.RequestFail -= OnRequestFail;

        FyberCallback.AdStarted -= OnAdStarted;
        FyberCallback.AdFinished -= OnAdFinished;

        FyberCallback.NativeError -= OnNativeExceptionReceivedFromSDK;
    }

    public void OnNativeExceptionReceivedFromSDK(string message)
    {
        //Handle Exception
        Debug.Log("Native exception error, wih message: " + message);
    }

    private void OnAdAvailable(Ad ad)
    {
        switch (ad.AdFormat)
        {
            case AdFormat.REWARDED_VIDEO:
                Debug.Log("Rewarded video available.");
                rewardedVideoAd = ad;
                break;
            case AdFormat.INTERSTITIAL:
                interstitialAd = ad;
                break;
            case AdFormat.OFFER_WALL:
                ofwAd = ad;
                break;
        }

        if (rewardedVideoAd != null)
        {
            rewardedVideoAd.Start();
            rewardedVideoAd = null;
        }
    }

    private void OnAdNotAvailable(AdFormat adFormat)
    {
        switch (adFormat)
        {
            case AdFormat.INTERSTITIAL:
                break;

            case AdFormat.REWARDED_VIDEO:
                Debug.Log("Rewarded Video Ad not available");
                break;
            
            case AdFormat.OFFER_WALL:
                break;
        }
    }

    private void OnAdStarted(Ad ad)
    {
        switch (ad.AdFormat)
        {
            case AdFormat.REWARDED_VIDEO:
                Debug.Log("Ad has started."); ;
                rewardedVideoAd = null;
                break;
                //handle other ad formats if needed
        }
    }

    private void OnAdFinished(AdResult result)
    {
        switch(result.AdFormat)
        {
            case AdFormat.REWARDED_VIDEO:
                Debug.Log("Rewarded video closed with result: " + result.Status + " and message: " + result.Message);
                break;
            // Handle other formats..
        }
    }

    private void OnRequestFail(RequestError error)
    {
        // Process Error
        UnityEngine.Debug.Log("OnRequestError: " + error.Description);
    }
}
