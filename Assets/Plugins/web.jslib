mergeInto(LibraryManager.library, {
  CloseApp: function () {
     window.CloseGameAWayOut();
  },

  ShowRewardedAds:  function(){
    window.ShowRewardedAds();
  },

  ShowRewardedAdsCoins:  function(coins){
    window.ShowRewardedAdsCoins(Pointer_stringify(coins));
  },

  ShowInterAds:  function(){
    window.ShowInterAds();
  }
});