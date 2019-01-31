# ImmGetCandidateListDemo
Demo of using `ImmGetCandidateList` from C# to retrieve hanzi candidates for a pinyin composition string.

Written to help add support for Chinese input to OptiKey in https://github.com/OptiKey/OptiKey/issues/515

Properly setting the composition string via `ImmSetCompositionString` remains a to-do. Currently am using the hacky method of `SendKeys`.

Developed and tested on Windows 10. There is notoriously a lot of variability in IMM support among Windows versions.
At minimum, the clearing of `ISC_SHOWUICANDIDATEWINDOW` needs to be put inside a test for >=Vista.

Have deliberately omitted Visual Studio config files.
To run, create a skeleton project and add these files.

![](ImmGetCandidateListDemo-screenie2.png)
