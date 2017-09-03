# dotnet-fast-route-experiment
Experimentation of a route lookup dictionary with character grouping

A test runs 5 million times a route lookup on a dictionary with 24 elements.

The baseline is a hash test which only works if the route is an exact match (url needs to be already split into segments):
MeasureActionRoute for HashDictionaryRouter with suffix ''
- animations1animations: 214
- categories1categories: 276
- categorytree1categorytree: 225
- group1group: 184
- grouptree1grouptree: 186
- limit1limit: 165
- list1list: 158
- login1login: 194
- version1version: 191
- versioninfo1versioninfo: 195
- xmlproperties1xmlproperties: 205
- xmlstructure1xmlstructure: 210
- animations2animations: 196
- categories2categories: 189
- categorytree2categorytree: 199
- group2group: 173
- grouptree2grouptree: 189
- limit2limit: 157
- list2list: 167
- login2login: 163
- version2version: 174
- versioninfo2versioninfo: 195
- xmlproperties2xmlproperties: 203
- xmlstructure2xmlstructure: 199
- notfound: 108

If all calls are mixed together, Modulo contains the overall average results for all routes tested (24 exact and 1 not found)

MeasureActionRouteModulo for HashDictionaryRouter
- total: 252

Fist lookup implementation is using Dictionary<char, ActionDictionary> to process consecutive characters and check the match.
This is considerably slower than a quick hash match.

Not using lookup
MeasureActionRouteModulo for ActionDictionaryRouter
- total: 794
MeasureActionRoute for ActionDictionaryRouter with suffix ''
- animations1animations: 555
- categories1categories: 794
- categorytree1categorytree: 784
- group1group: 527
- grouptree1grouptree: 786
- limit1limit: 859
- list1list: 838
- login1login: 682
- version1version: 533
- versioninfo1versioninfo: 797
- xmlproperties1xmlproperties: 808
- xmlstructure1xmlstructure: 794
- animations2animations: 559
- categories2categories: 792
- categorytree2categorytree: 803
- group2group: 522
- grouptree2grouptree: 784
- limit2limit: 860
- list2list: 870
- login2login: 682
- version2version: 541
- versioninfo2versioninfo: 817
- xmlproperties2xmlproperties: 819
- xmlstructure2xmlstructure: 837
- notfound: 29

As ActionDictionary router does not need exact matching, a suffix can be added to test urls which should not affect results (and it mostly does not):
MeasureActionRoute for ActionDictionaryRouter with suffix '?test'
- animations1animations: 562
- categories1categories: 787
- categorytree1categorytree: 797
- group1group: 519
- grouptree1grouptree: 776
- limit1limit: 857
- list1list: 847
- login1login: 688
- version1version: 568
- versioninfo1versioninfo: 784
- xmlproperties1xmlproperties: 809
- xmlstructure1xmlstructure: 794
- animations2animations: 563
- categories2categories: 802
- categorytree2categorytree: 796
- group2group: 533
- grouptree2grouptree: 794
- limit2limit: 858
- list2list: 855
- login2login: 687
- version2version: 544
- versioninfo2versioninfo: 782
- xmlproperties2xmlproperties: 803
- xmlstructure2xmlstructure: 797
- notfound: 164

Alternative implementation is using a lookup character array which will store 256 method references for each character allowing instant access - O(1).

Using lookup
MeasureActionRouteModulo for ActionDictionaryRouter
- total: 598
MeasureActionRoute for ActionDictionaryRouter with suffix ''
- animations1animations: 431
- categories1categories: 600
- categorytree1categorytree: 615
- group1group: 407
- grouptree1grouptree: 595
- limit1limit: 580
- list1list: 562
- login1login: 488
- version1version: 408
- versioninfo1versioninfo: 618
- xmlproperties1xmlproperties: 613
- xmlstructure1xmlstructure: 604
- animations2animations: 427
- categories2categories: 599
- categorytree2categorytree: 605
- group2group: 404
- grouptree2grouptree: 587
- limit2limit: 580
- list2list: 571
- login2login: 493
- version2version: 422
- versioninfo2versioninfo: 596
- xmlproperties2xmlproperties: 612
- xmlstructure2xmlstructure: 609
- notfound: 31
MeasureActionRoute for ActionDictionaryRouter with suffix '?test'
- animations1animations: 433
- categories1categories: 601
- categorytree1categorytree: 609
- group1group: 404
- grouptree1grouptree: 599
- limit1limit: 578
- list1list: 566
- login1login: 487
- version1version: 415
- versioninfo1versioninfo: 621
- xmlproperties1xmlproperties: 613
- xmlstructure1xmlstructure: 614
- animations2animations: 432
- categories2categories: 605
- categorytree2categorytree: 611
- group2group: 410
- grouptree2grouptree: 603
- limit2limit: 580
- list2list: 570
- login2login: 490
- version2version: 418
- versioninfo2versioninfo: 607
- xmlproperties2xmlproperties: 620
- xmlstructure2xmlstructure: 614
- notfound: 106

Unfortunately results are not satisfying since there is a need for a lot of calls to be made to framework functions including string char indexer:

            var c = status.Text[status.Index];
0DA0  push        rdi  
0DA1  push        rsi  
0DA2  sub         rsp,48h  
0DA6  xor         eax,eax  
0DA8  mov         qword ptr [rsp+40h],rax  
0DAD  mov         rdi,rcx  
0DB0  mov         rsi,rdx  
0DB3  mov         rdx,r8  
0DB6  mov         r8,qword ptr [rsi+8]  
0DBA  mov         ecx,dword ptr [rsi+14h]  
0DBD  cmp         ecx,dword ptr [r8+8]  
0DC1  jae         0EBF  
0DC7  movsxd      rcx,ecx  
0DCA  movzx       eax,word ptr [r8+rcx*2+0Ch]  
            if (dict.TryGetValue(c, out var result))
0DD0  lea         r8,[rsp+40h]  
0DD5  mov         rcx,rdx  
0DD8  mov         edx,eax  
0DDA  cmp         dword ptr [rcx],ecx  
0DDC  call        00007FFF7D6C0348  
0DE1  test        eax,eax  
0DE3  je          0EB6  
0DE9  mov         r9d,dword ptr [rsi+14h]  
0DED  inc         r9d  
0DF0  mov         dword ptr [rsi+14h],r9d  
0DF4  mov         ecx,dword ptr [rsi+10h]  
0DF7  mov         rdx,qword ptr [rsp+40h]  
0DFC  cmp         ecx,dword ptr [rdx+60h]  
0DFF  jl          0EB6  
                {
                    if (result.SuffixLength == 0 || status.Text.IndexOf(result.Suffix, status.Index, result.SuffixLength, StringComparison.Ordinal) >= 0)
0E05  mov         rcx,qword ptr [rsp+40h]  
                {
                    if (result.SuffixLength == 0 || status.Text.IndexOf(result.Suffix, status.Index, result.SuffixLength, StringComparison.Ordinal) >= 0)
0E0A  mov         ecx,dword ptr [rcx+64h]  
0E0D  test        ecx,ecx  
0E0F  je          0E44  
0E11  mov         rdx,qword ptr [rsi+8]  
0E15  mov         r8,qword ptr [rsp+40h]  
0E1A  mov         r8,qword ptr [r8+58h]  
0E1E  mov         dword ptr [rsp+3Ch],r9d  
0E23  mov         dword ptr [rsp+20h],4  
0E2B  mov         r9d,ecx  
0E2E  mov         rcx,rdx  
0E31  mov         rdx,r8  
0E34  mov         r8d,dword ptr [rsp+3Ch]  
0E39  cmp         dword ptr [rcx],ecx  
0E3B  call        00007FFFC8183710  
0E40  test        eax,eax  
0E42  jl          0EB6  
0E44  mov         ecx,dword ptr [rsi+14h]  
0E47  mov         rdx,qword ptr [rsp+40h]  
0E4C  add         ecx,dword ptr [rdx+64h]  
0E4F  mov         edx,ecx  
0E51  mov         dword ptr [rsi+14h],edx  
0E54  cmp         ecx,dword ptr [rsi+10h]  
0E57  jge         0E88  
                        if (status.Index < status.Length && result.Count > 0 && TestAction(status, result))
0E59  mov         rcx,qword ptr [rsp+40h]  
0E5E  mov         edx,dword ptr [rcx+38h]  
0E61  sub         edx,dword ptr [rcx+44h]  
0E64  test        edx,edx  
0E66  jle         0E88  
0E68  mov         rcx,rdi  
0E6B  mov         rdx,rsi  
0E6E  mov         r8,qword ptr [rsp+40h]  
0E73  call        00007FFF7D6C03A0  
0E78  test        eax,eax  
0E7A  je          0E88  
                        {
                            return true;
0E7C  mov         eax,1  
0E81  add         rsp,48h  
0E85  pop         rsi  
0E86  pop         rdi  
0E87  ret  
                        }
                        if (result.Action != null)
0E88  mov         rcx,qword ptr [rsp+40h]  
0E8D  mov         rcx,qword ptr [rcx+48h]  
0E91  test        rcx,rcx  
0E94  je          0EB6  
                        {
                            result.Action(status);
0E96  mov         qword ptr [rsp+30h],rcx  
0E9B  mov         rcx,qword ptr [rcx+8]  
0E9F  mov         rdx,rsi  
0EA2  mov         rax,qword ptr [rsp+30h]  
0EA7  call        qword ptr [rax+18h]  
                            return true;
0EAA  mov         eax,1  
0EAF  add         rsp,48h  
0EB3  pop         rsi  
0EB4  pop         rdi  
0EB5  ret  
                        }
                    }
                }
            }
            return false;
0EB6  xor         eax,eax  
0EB8  add         rsp,48h  
0EBC  pop         rsi  
0EBD  pop         rdi  
0EBE  ret  
0EBF  call        00007FFFDD3064D0  
0EC4  int         3  

This leads to the conclusion - it is more optimal to split the address
by known separatos (/, ?, ;) and do lookups using the standard dictionary object
which works on string hashes.
