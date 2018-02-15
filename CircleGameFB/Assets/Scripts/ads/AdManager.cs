﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;


public class AdManager : MonoBehaviour {

	[SerializeField]
	private GameObject adsPanel, failedToLoadAdGameObject;

	public static AdManager instance;

	string appID = "ca-app-pub-5218899435308484~7951254202";

	private InterstitialAd interstitialAd;
	private RewardBasedVideoAd rewardBasedAd;

	private bool adOpened;
	public bool AdOpened {  get { return adOpened; } }  


	void Awake() {
		if (instance == null)
			instance = this;

		adOpened = false; // we don t show any ad by default
	}

	void Start() {
		MobileAds.Initialize (appID);
		rewardBasedAd = RewardBasedVideoAd.Instance; // singleton
		rewardBasedAd.OnAdRewarded += HandleRewardBasedVideoRewarded;
		rewardBasedAd.OnAdOpening += HandleOnAdLoaded;
		rewardBasedAd.OnAdClosed += HandleOnAdClosed;
		rewardBasedAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;


		// request in the beggining and initialize for the first time the ads references
		RequestInterstitial ();
		RequestRewardedVideo ();
	}

	void OnDisable() {
		interstitialAd.Destroy (); // only the interstitial has to be destroyed, the reward ad it's of a singleton
	}


	private void RequestInterstitial() {

		//TODO change adUnitId
		#if UNITY_ANDROID
		string adUnitId = "ca-app-pub-3940256099942544/1033173712";
		#elif UNITY_IPHONE
		string adUnitId = "ca-app-pub-3940256099942544/4411468910";
		#else
		string adUnitId = "unexpected_platform";
		#endif

		// Initialize an InterstitialAd.
		interstitialAd = new InterstitialAd (adUnitId);

		//add events
		interstitialAd.OnAdOpening += HandleOnAdLoaded;
		interstitialAd.OnAdClosed += HandleOnAdClosed;
		interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;

		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder ()
		.TagForChildDirectedTreatment (true)
		.Build ();
		// Load the interstitial with the request.
		interstitialAd.LoadAd (request);

		}

		private void RequestRewardedVideo() {

		//TODO change adUnitId at RequestAd
		#if UNITY_ANDROID
		string adUnitId = "ca-app-pub-3940256099942544/5224354917";
		#elif UNITY_IPHONE
		string adUnitId = "ca-app-pub-3940256099942544/1712485313";
		#else
		string adUnitId = "unexpected_platform";
		#endif


		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder ()
		.Build ();
		// Load the rewarded video ad with the request.
		this.rewardBasedAd.LoadAd (request, adUnitId);

		}

		public void RequestAllAds() {

		if (interstitialAd != null) {
			if (!interstitialAd.IsLoaded ()) { // load only if it isn't loaded
				RequestInterstitial ();
			}
		}

		//if (!rewardBasedAd.IsLoaded ()) { // load only if it isn't loaded
		RequestRewardedVideo ();
		//}

		}

		public void ShowInterstitial() {

		if (interstitialAd != null)
		if (interstitialAd.IsLoaded ())
		interstitialAd.Show ();
		else
		RequestInterstitial ();
		}	

		public void ShowRewardAd() {

		if (rewardBasedAd.IsLoaded ())
		rewardBasedAd.Show ();
		else
		RequestRewardedVideo ();
		}

		public void HandleRewardBasedVideoRewarded(object sender, Reward args) {
		int coinAmount = (int) args.Amount;
		GamePreferences.SetCoinScore (GamePreferences.GetCoinScore () + coinAmount);
		}

		public void ShowUIAd() {
		GamePreferences.DecrementAdsCounter ();
		adOpened = true;
		float rnd = UnityEngine.Random.Range (0, 100);

		if (rnd < 50) // show rewardAd Panel
		adsPanel.SetActive (true);
		else // show interstitial Ad
		ShowInterstitial ();
		}

		public void YesButtonShowUIAd() {
		ShowRewardAd();
		adsPanel.SetActive (false);
		}

		public void NoButtonShowUIAd() {
		adsPanel.SetActive (false);
		}

		private void HandleOnAdLoaded(object sender, EventArgs args) {
		Time.timeScale = 0f;
		}

		private void HandleOnAdClosed(object sender, EventArgs args) {
		adOpened = false;
		Time.timeScale = 1f;
		}

		private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
		//TODO after the ads will work ad a logic here
		Debug.Log ("FAILED");
		}

		private IEnumerator ShowFailedToLoadAdText() {
		failedToLoadAdGameObject.SetActive (true);
		yield return new WaitForSeconds (1f);
		failedToLoadAdGameObject.SetActive (false);
		} 

		}