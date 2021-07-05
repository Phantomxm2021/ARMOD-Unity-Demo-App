//
//  NativeCallProxy.h
//  NativatePlugin
//
//  Created by phantomsxr on 2020/8/22.
//  Copyright © 2020 phantomsxr.com. All rights reserved.
//

#import <Foundation/Foundation.h>

// NativeCallsProtocol defines protocol with methods you want to be called from managed
@protocol NativeCallsProtocol

/*!
 @Discussion The method will be executed if an exception occurs
 @param message error string
 @param code error code of error type
*/

- (void)throwException:(NSString *)message errorCode:(int) code;
/*!
 @Discussion Exit the ARMOD
 */
@required
- (void)onARMODExit;

/*!
 @Discussion Start ARMOD
 */
@required
- (void)onARMODLaunch;
/*!
 @Discussion Start loading ARExperience
 */
@required
- (void)addLoadingOverlay;

/*!
 @Discussion Loading ARExperience asset progress
 @param progress the progress value of float type
 */
- (void) updateLoadingProgress:(float) progress;

/*!
 @Discussion Loading ARExperience finished
 */
@required
- (void)removeLoadingOverlay;


/*!
 @Discussion Current device is not support ARMOD
 */
@required
- (void)deviceNotSupport;

/*!
 @Discussion After the AR algorithm is initialized, the method will be executed
 */
@required
- (void)sdkInitialized;


/*!
 @Discussion Open the URL in a custom browser
 @param url URL  of NSString type
 */
@required
- (void)openBuiltInBrowser:(NSString *)url;


/*!
 @Discussion Start recognition
 */
@required
- (void)recognitionStart;


/*!
@Discussion Recognized successfully
 */
@required
- (void)recognitionComplete;

/*!
@Discussion Get app information from ARExperience script
@param opTag The type of operation request initiated by AR Experience.
 */
@required
- (NSString* )tryAcquireInformation:(NSString*) opTag;

/*!
@Discussion Detected that the ARExperience package is too large
@param currentSize current pacage size
@param presetSize Maximum downloadable package size
 */
@required
- (void )packageSizeMoreThanPresetSize:(float) currentSize preset:(float) presetSize;

@end

__attribute__ ((visibility("default")))
@interface FrameworkLibAPI : NSObject
/*!
 @Discussion call it any time after ARMODLOAD to set object implementing NativeCallsProtocol methods
 */
+(void) registerAPIforNativeCalls:(id<NativeCallsProtocol>) aApi;
@end
