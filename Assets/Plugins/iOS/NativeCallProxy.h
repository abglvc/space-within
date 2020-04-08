// [!] important set UnityFramework in Target Membership for this file
// [!]           and set Public header visibility

#import <Foundation/Foundation.h>

// NativeCallsProtocol defines protocol with methods you want to be called from managed
@protocol NativeCallsProtocol
@required
//example function
- (void) showHostMainWindow;

//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// x je trenutna vrijednost dubine ocitane sa senzora [0-8], jednostavna je i treba je implementovati u app-u
// nakon cega ona salje dalje ka Unity objektu F1SLink koji ima na sebi funkciju UpdateDepthFromSensor i proslijedit mu string
// jer samo tu vrstu podataka moze prenijeti kao poruku. 

// JAVA KOD
//	if (x != previousDepth) {
//		UnityPlayer.UnitySendMessage("F1SLink", "UpdateDepthFromSensor", x.toString());
//		previousDepth = x;
//	}

// obj c kod
// [[self ufw] sendMessageToGOWithName: "F1SLink" functionName: "UpdateDepthFromSensor" message: "X_TO_STRING"];
// gdje je ufw unityframeworkload

// sve detaljnije kako implementovati naci u primjeru za download "uaal-example" https://forum.unity.com/threads/integration-unity-as-a-library-in-native-ios-app.685219/
// proci kroz MainViewController.mm 
//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

- (void) updateDepthFromSensor:(NSInteger*)x;	
// other methods
@end

__attribute__ ((visibility("default")))
@interface FrameworkLibAPI : NSObject
// call it any time after UnityFrameworkLoad to set object implementing NativeCallsProtocol methods
+(void) registerAPIforNativeCalls:(id<NativeCallsProtocol>) aApi;

@end


