//
//  NativeCallProxy.mm
//  NativatePlugin
//
//  Created by phantomsxr on 2020/8/22.
//  Copyright © 2020 phantomsxr.com. All rights reserved.
//
#import <Foundation/Foundation.h>
#import "NativeCallProxy.h"


@implementation FrameworkLibAPI

id<NativeCallsProtocol> api = NULL;
+(void) registerAPIforNativeCalls:(id<NativeCallsProtocol>) aApi
{
    api = aApi;
}
@end


extern "C" {
    void throwException(const char *error, int errorCode){
        [api throwException:[NSString stringWithUTF8String:error] errorCode:errorCode];
    }

    void onARMODLaunch(){
        [api onARMODLaunch];
    }

    void onARMODExit(){
        [api onARMODExit];
    }

    void recognitionStart(){
        [api recognitionStart];
    }

    void recognitionComplete(){
        [api recognitionComplete];
    }

    void addLoadingOverlay(){
        [api addLoadingOverlay];
    }

    void removeLoadingOverlay(){
        [api removeLoadingOverlay];
    }

    void deviceNotSupport(){
        [api deviceNotSupport];
    }

    void updateLoadingProgress(float progress){
        [api updateLoadingProgress:progress];
    }

    void sdkInitialized(){
        [api sdkInitialized];
    }

    void openBuiltInBrowser(const char *url){
        [api openBuiltInBrowser:[NSString stringWithUTF8String:url]];
    }
    
    void packageSizeMoreThanPresetSize(float currentSize,float presetSize){
        [api packageSizeMoreThanPresetSize:currentSize preset:presetSize];
    }

    char* tryAcquireInformation(const char *opTag){
        NSString *info = [api tryAcquireInformation:[NSString stringWithUTF8String:opTag]];
        const char *info_encode =[info UTF8String];
        char* cString =(char*)malloc(strlen(info_encode)+1);
        strcpy(cString, info_encode);
        return cString;
    }
}
