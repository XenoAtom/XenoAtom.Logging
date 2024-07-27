// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

namespace XenoAtom.Logging.Writers;

/// <summary>
/// Defines the ANSI 256 colors.
/// </summary>
public static class ConsoleAnsiColors
{
    // Standard colors
    public static ReadOnlySpan<char> Black => "\u001b[38;5;0m";
    public static ReadOnlySpan<char> Maroon => "\u001b[38;5;1m";
    public static ReadOnlySpan<char> Green => "\u001b[38;5;2m";
    public static ReadOnlySpan<char> Olive => "\u001b[38;5;3m";
    public static ReadOnlySpan<char> Navy => "\u001b[38;5;4m";
    public static ReadOnlySpan<char> Purple => "\u001b[38;5;5m";
    public static ReadOnlySpan<char> Teal => "\u001b[38;5;6m";
    public static ReadOnlySpan<char> Silver => "\u001b[38;5;7m";
    public static ReadOnlySpan<char> Grey => "\u001b[38;5;8m";
    public static ReadOnlySpan<char> Red => "\u001b[38;5;9m";
    public static ReadOnlySpan<char> Lime => "\u001b[38;5;10m";
    public static ReadOnlySpan<char> Yellow => "\u001b[38;5;11m";
    public static ReadOnlySpan<char> Blue => "\u001b[38;5;12m";
    public static ReadOnlySpan<char> Fuchsia => "\u001b[38;5;13m";
    public static ReadOnlySpan<char> Aqua => "\u001b[38;5;14m";
    public static ReadOnlySpan<char> White => "\u001b[38;5;15m";

    // 256-color palette
    /// <summary>Color rgb(0, 0, 0)</summary>
    public static ReadOnlySpan<char> Color000 => "\x1b[38;5;0";
    /// <summary>Color rgb(128, 0, 0)</summary>
    public static ReadOnlySpan<char> Color001 => "\x1b[38;5;1";
    /// <summary>Color rgb(0, 128, 0)</summary>
    public static ReadOnlySpan<char> Color002 => "\x1b[38;5;2";
    /// <summary>Color rgb(128, 128, 0)</summary>
    public static ReadOnlySpan<char> Color003 => "\x1b[38;5;3";
    /// <summary>Color rgb(0, 0, 128)</summary>
    public static ReadOnlySpan<char> Color004 => "\x1b[38;5;4";
    /// <summary>Color rgb(128, 0, 128)</summary>
    public static ReadOnlySpan<char> Color005 => "\x1b[38;5;5";
    /// <summary>Color rgb(0, 128, 128)</summary>
    public static ReadOnlySpan<char> Color006 => "\x1b[38;5;6";
    /// <summary>Color rgb(192, 192, 192)</summary>
    public static ReadOnlySpan<char> Color007 => "\x1b[38;5;7";
    /// <summary>Color rgb(128, 128, 128)</summary>
    public static ReadOnlySpan<char> Color008 => "\x1b[38;5;8";
    /// <summary>Color rgb(255, 0, 0)</summary>
    public static ReadOnlySpan<char> Color009 => "\x1b[38;5;9";
    /// <summary>Color rgb(0, 255, 0)</summary>
    public static ReadOnlySpan<char> Color010 => "\x1b[38;5;10";
    /// <summary>Color rgb(255, 255, 0)</summary>
    public static ReadOnlySpan<char> Color011 => "\x1b[38;5;11";
    /// <summary>Color rgb(0, 0, 255)</summary>
    public static ReadOnlySpan<char> Color012 => "\x1b[38;5;12";
    /// <summary>Color rgb(255, 0, 255)</summary>
    public static ReadOnlySpan<char> Color013 => "\x1b[38;5;13";
    /// <summary>Color rgb(0, 255, 255)</summary>
    public static ReadOnlySpan<char> Color014 => "\x1b[38;5;14";
    /// <summary>Color rgb(255, 255, 255)</summary>
    public static ReadOnlySpan<char> Color015 => "\x1b[38;5;15";
    /// <summary>Color rgb(0, 0, 0)</summary>
    public static ReadOnlySpan<char> Color016 => "\x1b[38;5;16";
    /// <summary>Color rgb(0, 0, 95)</summary>
    public static ReadOnlySpan<char> Color017 => "\x1b[38;5;17";
    /// <summary>Color rgb(0, 0, 135)</summary>
    public static ReadOnlySpan<char> Color018 => "\x1b[38;5;18";
    /// <summary>Color rgb(0, 0, 175)</summary>
    public static ReadOnlySpan<char> Color019 => "\x1b[38;5;19";
    /// <summary>Color rgb(0, 0, 215)</summary>
    public static ReadOnlySpan<char> Color020 => "\x1b[38;5;20";
    /// <summary>Color rgb(0, 0, 255)</summary>
    public static ReadOnlySpan<char> Color021 => "\x1b[38;5;21";
    /// <summary>Color rgb(0, 95, 0)</summary>
    public static ReadOnlySpan<char> Color022 => "\x1b[38;5;22";
    /// <summary>Color rgb(0, 95, 95)</summary>
    public static ReadOnlySpan<char> Color023 => "\x1b[38;5;23";
    /// <summary>Color rgb(0, 95, 135)</summary>
    public static ReadOnlySpan<char> Color024 => "\x1b[38;5;24";
    /// <summary>Color rgb(0, 95, 175)</summary>
    public static ReadOnlySpan<char> Color025 => "\x1b[38;5;25";
    /// <summary>Color rgb(0, 95, 215)</summary>
    public static ReadOnlySpan<char> Color026 => "\x1b[38;5;26";
    /// <summary>Color rgb(0, 95, 255)</summary>
    public static ReadOnlySpan<char> Color027 => "\x1b[38;5;27";
    /// <summary>Color rgb(0, 135, 0)</summary>
    public static ReadOnlySpan<char> Color028 => "\x1b[38;5;28";
    /// <summary>Color rgb(0, 135, 95)</summary>
    public static ReadOnlySpan<char> Color029 => "\x1b[38;5;29";
    /// <summary>Color rgb(0, 135, 135)</summary>
    public static ReadOnlySpan<char> Color030 => "\x1b[38;5;30";
    /// <summary>Color rgb(0, 135, 175)</summary>
    public static ReadOnlySpan<char> Color031 => "\x1b[38;5;31";
    /// <summary>Color rgb(0, 135, 215)</summary>
    public static ReadOnlySpan<char> Color032 => "\x1b[38;5;32";
    /// <summary>Color rgb(0, 135, 255)</summary>
    public static ReadOnlySpan<char> Color033 => "\x1b[38;5;33";
    /// <summary>Color rgb(0, 175, 0)</summary>
    public static ReadOnlySpan<char> Color034 => "\x1b[38;5;34";
    /// <summary>Color rgb(0, 175, 95)</summary>
    public static ReadOnlySpan<char> Color035 => "\x1b[38;5;35";
    /// <summary>Color rgb(0, 175, 135)</summary>
    public static ReadOnlySpan<char> Color036 => "\x1b[38;5;36";
    /// <summary>Color rgb(0, 175, 175)</summary>
    public static ReadOnlySpan<char> Color037 => "\x1b[38;5;37";
    /// <summary>Color rgb(0, 175, 215)</summary>
    public static ReadOnlySpan<char> Color038 => "\x1b[38;5;38";
    /// <summary>Color rgb(0, 175, 255)</summary>
    public static ReadOnlySpan<char> Color039 => "\x1b[38;5;39";
    /// <summary>Color rgb(0, 215, 0)</summary>
    public static ReadOnlySpan<char> Color040 => "\x1b[38;5;40";
    /// <summary>Color rgb(0, 215, 95)</summary>
    public static ReadOnlySpan<char> Color041 => "\x1b[38;5;41";
    /// <summary>Color rgb(0, 215, 135)</summary>
    public static ReadOnlySpan<char> Color042 => "\x1b[38;5;42";
    /// <summary>Color rgb(0, 215, 175)</summary>
    public static ReadOnlySpan<char> Color043 => "\x1b[38;5;43";
    /// <summary>Color rgb(0, 215, 215)</summary>
    public static ReadOnlySpan<char> Color044 => "\x1b[38;5;44";
    /// <summary>Color rgb(0, 215, 255)</summary>
    public static ReadOnlySpan<char> Color045 => "\x1b[38;5;45";
    /// <summary>Color rgb(0, 255, 0)</summary>
    public static ReadOnlySpan<char> Color046 => "\x1b[38;5;46";
    /// <summary>Color rgb(0, 255, 95)</summary>
    public static ReadOnlySpan<char> Color047 => "\x1b[38;5;47";
    /// <summary>Color rgb(0, 255, 135)</summary>
    public static ReadOnlySpan<char> Color048 => "\x1b[38;5;48";
    /// <summary>Color rgb(0, 255, 175)</summary>
    public static ReadOnlySpan<char> Color049 => "\x1b[38;5;49";
    /// <summary>Color rgb(0, 255, 215)</summary>
    public static ReadOnlySpan<char> Color050 => "\x1b[38;5;50";
    /// <summary>Color rgb(0, 255, 255)</summary>
    public static ReadOnlySpan<char> Color051 => "\x1b[38;5;51";
    /// <summary>Color rgb(95, 0, 0)</summary>
    public static ReadOnlySpan<char> Color052 => "\x1b[38;5;52";
    /// <summary>Color rgb(95, 0, 95)</summary>
    public static ReadOnlySpan<char> Color053 => "\x1b[38;5;53";
    /// <summary>Color rgb(95, 0, 135)</summary>
    public static ReadOnlySpan<char> Color054 => "\x1b[38;5;54";
    /// <summary>Color rgb(95, 0, 175)</summary>
    public static ReadOnlySpan<char> Color055 => "\x1b[38;5;55";
    /// <summary>Color rgb(95, 0, 215)</summary>
    public static ReadOnlySpan<char> Color056 => "\x1b[38;5;56";
    /// <summary>Color rgb(95, 0, 255)</summary>
    public static ReadOnlySpan<char> Color057 => "\x1b[38;5;57";
    /// <summary>Color rgb(95, 95, 0)</summary>
    public static ReadOnlySpan<char> Color058 => "\x1b[38;5;58";
    /// <summary>Color rgb(95, 95, 95)</summary>
    public static ReadOnlySpan<char> Color059 => "\x1b[38;5;59";
    /// <summary>Color rgb(95, 95, 135)</summary>
    public static ReadOnlySpan<char> Color060 => "\x1b[38;5;60";
    /// <summary>Color rgb(95, 95, 175)</summary>
    public static ReadOnlySpan<char> Color061 => "\x1b[38;5;61";
    /// <summary>Color rgb(95, 95, 215)</summary>
    public static ReadOnlySpan<char> Color062 => "\x1b[38;5;62";
    /// <summary>Color rgb(95, 95, 255)</summary>
    public static ReadOnlySpan<char> Color063 => "\x1b[38;5;63";
    /// <summary>Color rgb(95, 135, 0)</summary>
    public static ReadOnlySpan<char> Color064 => "\x1b[38;5;64";
    /// <summary>Color rgb(95, 135, 95)</summary>
    public static ReadOnlySpan<char> Color065 => "\x1b[38;5;65";
    /// <summary>Color rgb(95, 135, 135)</summary>
    public static ReadOnlySpan<char> Color066 => "\x1b[38;5;66";
    /// <summary>Color rgb(95, 135, 175)</summary>
    public static ReadOnlySpan<char> Color067 => "\x1b[38;5;67";
    /// <summary>Color rgb(95, 135, 215)</summary>
    public static ReadOnlySpan<char> Color068 => "\x1b[38;5;68";
    /// <summary>Color rgb(95, 135, 255)</summary>
    public static ReadOnlySpan<char> Color069 => "\x1b[38;5;69";
    /// <summary>Color rgb(95, 175, 0)</summary>
    public static ReadOnlySpan<char> Color070 => "\x1b[38;5;70";
    /// <summary>Color rgb(95, 175, 95)</summary>
    public static ReadOnlySpan<char> Color071 => "\x1b[38;5;71";
    /// <summary>Color rgb(95, 175, 135)</summary>
    public static ReadOnlySpan<char> Color072 => "\x1b[38;5;72";
    /// <summary>Color rgb(95, 175, 175)</summary>
    public static ReadOnlySpan<char> Color073 => "\x1b[38;5;73";
    /// <summary>Color rgb(95, 175, 215)</summary>
    public static ReadOnlySpan<char> Color074 => "\x1b[38;5;74";
    /// <summary>Color rgb(95, 175, 255)</summary>
    public static ReadOnlySpan<char> Color075 => "\x1b[38;5;75";
    /// <summary>Color rgb(95, 215, 0)</summary>
    public static ReadOnlySpan<char> Color076 => "\x1b[38;5;76";
    /// <summary>Color rgb(95, 215, 95)</summary>
    public static ReadOnlySpan<char> Color077 => "\x1b[38;5;77";
    /// <summary>Color rgb(95, 215, 135)</summary>
    public static ReadOnlySpan<char> Color078 => "\x1b[38;5;78";
    /// <summary>Color rgb(95, 215, 175)</summary>
    public static ReadOnlySpan<char> Color079 => "\x1b[38;5;79";
    /// <summary>Color rgb(95, 215, 215)</summary>
    public static ReadOnlySpan<char> Color080 => "\x1b[38;5;80";
    /// <summary>Color rgb(95, 215, 255)</summary>
    public static ReadOnlySpan<char> Color081 => "\x1b[38;5;81";
    /// <summary>Color rgb(95, 255, 0)</summary>
    public static ReadOnlySpan<char> Color082 => "\x1b[38;5;82";
    /// <summary>Color rgb(95, 255, 95)</summary>
    public static ReadOnlySpan<char> Color083 => "\x1b[38;5;83";
    /// <summary>Color rgb(95, 255, 135)</summary>
    public static ReadOnlySpan<char> Color084 => "\x1b[38;5;84";
    /// <summary>Color rgb(95, 255, 175)</summary>
    public static ReadOnlySpan<char> Color085 => "\x1b[38;5;85";
    /// <summary>Color rgb(95, 255, 215)</summary>
    public static ReadOnlySpan<char> Color086 => "\x1b[38;5;86";
    /// <summary>Color rgb(95, 255, 255)</summary>
    public static ReadOnlySpan<char> Color087 => "\x1b[38;5;87";
    /// <summary>Color rgb(135, 0, 0)</summary>
    public static ReadOnlySpan<char> Color088 => "\x1b[38;5;88";
    /// <summary>Color rgb(135, 0, 95)</summary>
    public static ReadOnlySpan<char> Color089 => "\x1b[38;5;89";
    /// <summary>Color rgb(135, 0, 135)</summary>
    public static ReadOnlySpan<char> Color090 => "\x1b[38;5;90";
    /// <summary>Color rgb(135, 0, 175)</summary>
    public static ReadOnlySpan<char> Color091 => "\x1b[38;5;91";
    /// <summary>Color rgb(135, 0, 215)</summary>
    public static ReadOnlySpan<char> Color092 => "\x1b[38;5;92";
    /// <summary>Color rgb(135, 0, 255)</summary>
    public static ReadOnlySpan<char> Color093 => "\x1b[38;5;93";
    /// <summary>Color rgb(135, 95, 0)</summary>
    public static ReadOnlySpan<char> Color094 => "\x1b[38;5;94";
    /// <summary>Color rgb(135, 95, 95)</summary>
    public static ReadOnlySpan<char> Color095 => "\x1b[38;5;95";
    /// <summary>Color rgb(135, 95, 135)</summary>
    public static ReadOnlySpan<char> Color096 => "\x1b[38;5;96";
    /// <summary>Color rgb(135, 95, 175)</summary>
    public static ReadOnlySpan<char> Color097 => "\x1b[38;5;97";
    /// <summary>Color rgb(135, 95, 215)</summary>
    public static ReadOnlySpan<char> Color098 => "\x1b[38;5;98";
    /// <summary>Color rgb(135, 95, 255)</summary>
    public static ReadOnlySpan<char> Color099 => "\x1b[38;5;99";
    /// <summary>Color rgb(135, 135, 0)</summary>
    public static ReadOnlySpan<char> Color100 => "\x1b[38;5;100";
    /// <summary>Color rgb(135, 135, 95)</summary>
    public static ReadOnlySpan<char> Color101 => "\x1b[38;5;101";
    /// <summary>Color rgb(135, 135, 135)</summary>
    public static ReadOnlySpan<char> Color102 => "\x1b[38;5;102";
    /// <summary>Color rgb(135, 135, 175)</summary>
    public static ReadOnlySpan<char> Color103 => "\x1b[38;5;103";
    /// <summary>Color rgb(135, 135, 215)</summary>
    public static ReadOnlySpan<char> Color104 => "\x1b[38;5;104";
    /// <summary>Color rgb(135, 135, 255)</summary>
    public static ReadOnlySpan<char> Color105 => "\x1b[38;5;105";
    /// <summary>Color rgb(135, 175, 0)</summary>
    public static ReadOnlySpan<char> Color106 => "\x1b[38;5;106";
    /// <summary>Color rgb(135, 175, 95)</summary>
    public static ReadOnlySpan<char> Color107 => "\x1b[38;5;107";
    /// <summary>Color rgb(135, 175, 135)</summary>
    public static ReadOnlySpan<char> Color108 => "\x1b[38;5;108";
    /// <summary>Color rgb(135, 175, 175)</summary>
    public static ReadOnlySpan<char> Color109 => "\x1b[38;5;109";
    /// <summary>Color rgb(135, 175, 215)</summary>
    public static ReadOnlySpan<char> Color110 => "\x1b[38;5;110";
    /// <summary>Color rgb(135, 175, 255)</summary>
    public static ReadOnlySpan<char> Color111 => "\x1b[38;5;111";
    /// <summary>Color rgb(135, 215, 0)</summary>
    public static ReadOnlySpan<char> Color112 => "\x1b[38;5;112";
    /// <summary>Color rgb(135, 215, 95)</summary>
    public static ReadOnlySpan<char> Color113 => "\x1b[38;5;113";
    /// <summary>Color rgb(135, 215, 135)</summary>
    public static ReadOnlySpan<char> Color114 => "\x1b[38;5;114";
    /// <summary>Color rgb(135, 215, 175)</summary>
    public static ReadOnlySpan<char> Color115 => "\x1b[38;5;115";
    /// <summary>Color rgb(135, 215, 215)</summary>
    public static ReadOnlySpan<char> Color116 => "\x1b[38;5;116";
    /// <summary>Color rgb(135, 215, 255)</summary>
    public static ReadOnlySpan<char> Color117 => "\x1b[38;5;117";
    /// <summary>Color rgb(135, 255, 0)</summary>
    public static ReadOnlySpan<char> Color118 => "\x1b[38;5;118";
    /// <summary>Color rgb(135, 255, 95)</summary>
    public static ReadOnlySpan<char> Color119 => "\x1b[38;5;119";
    /// <summary>Color rgb(135, 255, 135)</summary>
    public static ReadOnlySpan<char> Color120 => "\x1b[38;5;120";
    /// <summary>Color rgb(135, 255, 175)</summary>
    public static ReadOnlySpan<char> Color121 => "\x1b[38;5;121";
    /// <summary>Color rgb(135, 255, 215)</summary>
    public static ReadOnlySpan<char> Color122 => "\x1b[38;5;122";
    /// <summary>Color rgb(135, 255, 255)</summary>
    public static ReadOnlySpan<char> Color123 => "\x1b[38;5;123";
    /// <summary>Color rgb(175, 0, 0)</summary>
    public static ReadOnlySpan<char> Color124 => "\x1b[38;5;124";
    /// <summary>Color rgb(175, 0, 95)</summary>
    public static ReadOnlySpan<char> Color125 => "\x1b[38;5;125";
    /// <summary>Color rgb(175, 0, 135)</summary>
    public static ReadOnlySpan<char> Color126 => "\x1b[38;5;126";
    /// <summary>Color rgb(175, 0, 175)</summary>
    public static ReadOnlySpan<char> Color127 => "\x1b[38;5;127";
    /// <summary>Color rgb(175, 0, 215)</summary>
    public static ReadOnlySpan<char> Color128 => "\x1b[38;5;128";
    /// <summary>Color rgb(175, 0, 255)</summary>
    public static ReadOnlySpan<char> Color129 => "\x1b[38;5;129";
    /// <summary>Color rgb(175, 95, 0)</summary>
    public static ReadOnlySpan<char> Color130 => "\x1b[38;5;130";
    /// <summary>Color rgb(175, 95, 95)</summary>
    public static ReadOnlySpan<char> Color131 => "\x1b[38;5;131";
    /// <summary>Color rgb(175, 95, 135)</summary>
    public static ReadOnlySpan<char> Color132 => "\x1b[38;5;132";
    /// <summary>Color rgb(175, 95, 175)</summary>
    public static ReadOnlySpan<char> Color133 => "\x1b[38;5;133";
    /// <summary>Color rgb(175, 95, 215)</summary>
    public static ReadOnlySpan<char> Color134 => "\x1b[38;5;134";
    /// <summary>Color rgb(175, 95, 255)</summary>
    public static ReadOnlySpan<char> Color135 => "\x1b[38;5;135";
    /// <summary>Color rgb(175, 135, 0)</summary>
    public static ReadOnlySpan<char> Color136 => "\x1b[38;5;136";
    /// <summary>Color rgb(175, 135, 95)</summary>
    public static ReadOnlySpan<char> Color137 => "\x1b[38;5;137";
    /// <summary>Color rgb(175, 135, 135)</summary>
    public static ReadOnlySpan<char> Color138 => "\x1b[38;5;138";
    /// <summary>Color rgb(175, 135, 175)</summary>
    public static ReadOnlySpan<char> Color139 => "\x1b[38;5;139";
    /// <summary>Color rgb(175, 135, 215)</summary>
    public static ReadOnlySpan<char> Color140 => "\x1b[38;5;140";
    /// <summary>Color rgb(175, 135, 255)</summary>
    public static ReadOnlySpan<char> Color141 => "\x1b[38;5;141";
    /// <summary>Color rgb(175, 175, 0)</summary>
    public static ReadOnlySpan<char> Color142 => "\x1b[38;5;142";
    /// <summary>Color rgb(175, 175, 95)</summary>
    public static ReadOnlySpan<char> Color143 => "\x1b[38;5;143";
    /// <summary>Color rgb(175, 175, 135)</summary>
    public static ReadOnlySpan<char> Color144 => "\x1b[38;5;144";
    /// <summary>Color rgb(175, 175, 175)</summary>
    public static ReadOnlySpan<char> Color145 => "\x1b[38;5;145";
    /// <summary>Color rgb(175, 175, 215)</summary>
    public static ReadOnlySpan<char> Color146 => "\x1b[38;5;146";
    /// <summary>Color rgb(175, 175, 255)</summary>
    public static ReadOnlySpan<char> Color147 => "\x1b[38;5;147";
    /// <summary>Color rgb(175, 215, 0)</summary>
    public static ReadOnlySpan<char> Color148 => "\x1b[38;5;148";
    /// <summary>Color rgb(175, 215, 95)</summary>
    public static ReadOnlySpan<char> Color149 => "\x1b[38;5;149";
    /// <summary>Color rgb(175, 215, 135)</summary>
    public static ReadOnlySpan<char> Color150 => "\x1b[38;5;150";
    /// <summary>Color rgb(175, 215, 175)</summary>
    public static ReadOnlySpan<char> Color151 => "\x1b[38;5;151";
    /// <summary>Color rgb(175, 215, 215)</summary>
    public static ReadOnlySpan<char> Color152 => "\x1b[38;5;152";
    /// <summary>Color rgb(175, 215, 255)</summary>
    public static ReadOnlySpan<char> Color153 => "\x1b[38;5;153";
    /// <summary>Color rgb(175, 255, 0)</summary>
    public static ReadOnlySpan<char> Color154 => "\x1b[38;5;154";
    /// <summary>Color rgb(175, 255, 95)</summary>
    public static ReadOnlySpan<char> Color155 => "\x1b[38;5;155";
    /// <summary>Color rgb(175, 255, 135)</summary>
    public static ReadOnlySpan<char> Color156 => "\x1b[38;5;156";
    /// <summary>Color rgb(175, 255, 175)</summary>
    public static ReadOnlySpan<char> Color157 => "\x1b[38;5;157";
    /// <summary>Color rgb(175, 255, 215)</summary>
    public static ReadOnlySpan<char> Color158 => "\x1b[38;5;158";
    /// <summary>Color rgb(175, 255, 255)</summary>
    public static ReadOnlySpan<char> Color159 => "\x1b[38;5;159";
    /// <summary>Color rgb(215, 0, 0)</summary>
    public static ReadOnlySpan<char> Color160 => "\x1b[38;5;160";
    /// <summary>Color rgb(215, 0, 95)</summary>
    public static ReadOnlySpan<char> Color161 => "\x1b[38;5;161";
    /// <summary>Color rgb(215, 0, 135)</summary>
    public static ReadOnlySpan<char> Color162 => "\x1b[38;5;162";
    /// <summary>Color rgb(215, 0, 175)</summary>
    public static ReadOnlySpan<char> Color163 => "\x1b[38;5;163";
    /// <summary>Color rgb(215, 0, 215)</summary>
    public static ReadOnlySpan<char> Color164 => "\x1b[38;5;164";
    /// <summary>Color rgb(215, 0, 255)</summary>
    public static ReadOnlySpan<char> Color165 => "\x1b[38;5;165";
    /// <summary>Color rgb(215, 95, 0)</summary>
    public static ReadOnlySpan<char> Color166 => "\x1b[38;5;166";
    /// <summary>Color rgb(215, 95, 95)</summary>
    public static ReadOnlySpan<char> Color167 => "\x1b[38;5;167";
    /// <summary>Color rgb(215, 95, 135)</summary>
    public static ReadOnlySpan<char> Color168 => "\x1b[38;5;168";
    /// <summary>Color rgb(215, 95, 175)</summary>
    public static ReadOnlySpan<char> Color169 => "\x1b[38;5;169";
    /// <summary>Color rgb(215, 95, 215)</summary>
    public static ReadOnlySpan<char> Color170 => "\x1b[38;5;170";
    /// <summary>Color rgb(215, 95, 255)</summary>
    public static ReadOnlySpan<char> Color171 => "\x1b[38;5;171";
    /// <summary>Color rgb(215, 135, 0)</summary>
    public static ReadOnlySpan<char> Color172 => "\x1b[38;5;172";
    /// <summary>Color rgb(215, 135, 95)</summary>
    public static ReadOnlySpan<char> Color173 => "\x1b[38;5;173";
    /// <summary>Color rgb(215, 135, 135)</summary>
    public static ReadOnlySpan<char> Color174 => "\x1b[38;5;174";
    /// <summary>Color rgb(215, 135, 175)</summary>
    public static ReadOnlySpan<char> Color175 => "\x1b[38;5;175";
    /// <summary>Color rgb(215, 135, 215)</summary>
    public static ReadOnlySpan<char> Color176 => "\x1b[38;5;176";
    /// <summary>Color rgb(215, 135, 255)</summary>
    public static ReadOnlySpan<char> Color177 => "\x1b[38;5;177";
    /// <summary>Color rgb(215, 175, 0)</summary>
    public static ReadOnlySpan<char> Color178 => "\x1b[38;5;178";
    /// <summary>Color rgb(215, 175, 95)</summary>
    public static ReadOnlySpan<char> Color179 => "\x1b[38;5;179";
    /// <summary>Color rgb(215, 175, 135)</summary>
    public static ReadOnlySpan<char> Color180 => "\x1b[38;5;180";
    /// <summary>Color rgb(215, 175, 175)</summary>
    public static ReadOnlySpan<char> Color181 => "\x1b[38;5;181";
    /// <summary>Color rgb(215, 175, 215)</summary>
    public static ReadOnlySpan<char> Color182 => "\x1b[38;5;182";
    /// <summary>Color rgb(215, 175, 255)</summary>
    public static ReadOnlySpan<char> Color183 => "\x1b[38;5;183";
    /// <summary>Color rgb(215, 215, 0)</summary>
    public static ReadOnlySpan<char> Color184 => "\x1b[38;5;184";
    /// <summary>Color rgb(215, 215, 95)</summary>
    public static ReadOnlySpan<char> Color185 => "\x1b[38;5;185";
    /// <summary>Color rgb(215, 215, 135)</summary>
    public static ReadOnlySpan<char> Color186 => "\x1b[38;5;186";
    /// <summary>Color rgb(215, 215, 175)</summary>
    public static ReadOnlySpan<char> Color187 => "\x1b[38;5;187";
    /// <summary>Color rgb(215, 215, 215)</summary>
    public static ReadOnlySpan<char> Color188 => "\x1b[38;5;188";
    /// <summary>Color rgb(215, 215, 255)</summary>
    public static ReadOnlySpan<char> Color189 => "\x1b[38;5;189";
    /// <summary>Color rgb(215, 255, 0)</summary>
    public static ReadOnlySpan<char> Color190 => "\x1b[38;5;190";
    /// <summary>Color rgb(215, 255, 95)</summary>
    public static ReadOnlySpan<char> Color191 => "\x1b[38;5;191";
    /// <summary>Color rgb(215, 255, 135)</summary>
    public static ReadOnlySpan<char> Color192 => "\x1b[38;5;192";
    /// <summary>Color rgb(215, 255, 175)</summary>
    public static ReadOnlySpan<char> Color193 => "\x1b[38;5;193";
    /// <summary>Color rgb(215, 255, 215)</summary>
    public static ReadOnlySpan<char> Color194 => "\x1b[38;5;194";
    /// <summary>Color rgb(215, 255, 255)</summary>
    public static ReadOnlySpan<char> Color195 => "\x1b[38;5;195";
    /// <summary>Color rgb(255, 0, 0)</summary>
    public static ReadOnlySpan<char> Color196 => "\x1b[38;5;196";
    /// <summary>Color rgb(255, 0, 95)</summary>
    public static ReadOnlySpan<char> Color197 => "\x1b[38;5;197";
    /// <summary>Color rgb(255, 0, 135)</summary>
    public static ReadOnlySpan<char> Color198 => "\x1b[38;5;198";
    /// <summary>Color rgb(255, 0, 175)</summary>
    public static ReadOnlySpan<char> Color199 => "\x1b[38;5;199";
    /// <summary>Color rgb(255, 0, 215)</summary>
    public static ReadOnlySpan<char> Color200 => "\x1b[38;5;200";
    /// <summary>Color rgb(255, 0, 255)</summary>
    public static ReadOnlySpan<char> Color201 => "\x1b[38;5;201";
    /// <summary>Color rgb(255, 95, 0)</summary>
    public static ReadOnlySpan<char> Color202 => "\x1b[38;5;202";
    /// <summary>Color rgb(255, 95, 95)</summary>
    public static ReadOnlySpan<char> Color203 => "\x1b[38;5;203";
    /// <summary>Color rgb(255, 95, 135)</summary>
    public static ReadOnlySpan<char> Color204 => "\x1b[38;5;204";
    /// <summary>Color rgb(255, 95, 175)</summary>
    public static ReadOnlySpan<char> Color205 => "\x1b[38;5;205";
    /// <summary>Color rgb(255, 95, 215)</summary>
    public static ReadOnlySpan<char> Color206 => "\x1b[38;5;206";
    /// <summary>Color rgb(255, 95, 255)</summary>
    public static ReadOnlySpan<char> Color207 => "\x1b[38;5;207";
    /// <summary>Color rgb(255, 135, 0)</summary>
    public static ReadOnlySpan<char> Color208 => "\x1b[38;5;208";
    /// <summary>Color rgb(255, 135, 95)</summary>
    public static ReadOnlySpan<char> Color209 => "\x1b[38;5;209";
    /// <summary>Color rgb(255, 135, 135)</summary>
    public static ReadOnlySpan<char> Color210 => "\x1b[38;5;210";
    /// <summary>Color rgb(255, 135, 175)</summary>
    public static ReadOnlySpan<char> Color211 => "\x1b[38;5;211";
    /// <summary>Color rgb(255, 135, 215)</summary>
    public static ReadOnlySpan<char> Color212 => "\x1b[38;5;212";
    /// <summary>Color rgb(255, 135, 255)</summary>
    public static ReadOnlySpan<char> Color213 => "\x1b[38;5;213";
    /// <summary>Color rgb(255, 175, 0)</summary>
    public static ReadOnlySpan<char> Color214 => "\x1b[38;5;214";
    /// <summary>Color rgb(255, 175, 95)</summary>
    public static ReadOnlySpan<char> Color215 => "\x1b[38;5;215";
    /// <summary>Color rgb(255, 175, 135)</summary>
    public static ReadOnlySpan<char> Color216 => "\x1b[38;5;216";
    /// <summary>Color rgb(255, 175, 175)</summary>
    public static ReadOnlySpan<char> Color217 => "\x1b[38;5;217";
    /// <summary>Color rgb(255, 175, 215)</summary>
    public static ReadOnlySpan<char> Color218 => "\x1b[38;5;218";
    /// <summary>Color rgb(255, 175, 255)</summary>
    public static ReadOnlySpan<char> Color219 => "\x1b[38;5;219";
    /// <summary>Color rgb(255, 215, 0)</summary>
    public static ReadOnlySpan<char> Color220 => "\x1b[38;5;220";
    /// <summary>Color rgb(255, 215, 95)</summary>
    public static ReadOnlySpan<char> Color221 => "\x1b[38;5;221";
    /// <summary>Color rgb(255, 215, 135)</summary>
    public static ReadOnlySpan<char> Color222 => "\x1b[38;5;222";
    /// <summary>Color rgb(255, 215, 175)</summary>
    public static ReadOnlySpan<char> Color223 => "\x1b[38;5;223";
    /// <summary>Color rgb(255, 215, 215)</summary>
    public static ReadOnlySpan<char> Color224 => "\x1b[38;5;224";
    /// <summary>Color rgb(255, 215, 255)</summary>
    public static ReadOnlySpan<char> Color225 => "\x1b[38;5;225";
    /// <summary>Color rgb(255, 255, 0)</summary>
    public static ReadOnlySpan<char> Color226 => "\x1b[38;5;226";
    /// <summary>Color rgb(255, 255, 95)</summary>
    public static ReadOnlySpan<char> Color227 => "\x1b[38;5;227";
    /// <summary>Color rgb(255, 255, 135)</summary>
    public static ReadOnlySpan<char> Color228 => "\x1b[38;5;228";
    /// <summary>Color rgb(255, 255, 175)</summary>
    public static ReadOnlySpan<char> Color229 => "\x1b[38;5;229";
    /// <summary>Color rgb(255, 255, 215)</summary>
    public static ReadOnlySpan<char> Color230 => "\x1b[38;5;230";
    /// <summary>Color rgb(255, 255, 255)</summary>
    public static ReadOnlySpan<char> Color231 => "\x1b[38;5;231";
    /// <summary>Color rgb(8, 8, 8)</summary>
    public static ReadOnlySpan<char> Color232 => "\x1b[38;5;232";
    /// <summary>Color rgb(18, 18, 18)</summary>
    public static ReadOnlySpan<char> Color233 => "\x1b[38;5;233";
    /// <summary>Color rgb(28, 28, 28)</summary>
    public static ReadOnlySpan<char> Color234 => "\x1b[38;5;234";
    /// <summary>Color rgb(38, 38, 38)</summary>
    public static ReadOnlySpan<char> Color235 => "\x1b[38;5;235";
    /// <summary>Color rgb(48, 48, 48)</summary>
    public static ReadOnlySpan<char> Color236 => "\x1b[38;5;236";
    /// <summary>Color rgb(58, 58, 58)</summary>
    public static ReadOnlySpan<char> Color237 => "\x1b[38;5;237";
    /// <summary>Color rgb(68, 68, 68)</summary>
    public static ReadOnlySpan<char> Color238 => "\x1b[38;5;238";
    /// <summary>Color rgb(78, 78, 78)</summary>
    public static ReadOnlySpan<char> Color239 => "\x1b[38;5;239";
    /// <summary>Color rgb(88, 88, 88)</summary>
    public static ReadOnlySpan<char> Color240 => "\x1b[38;5;240";
    /// <summary>Color rgb(98, 98, 98)</summary>
    public static ReadOnlySpan<char> Color241 => "\x1b[38;5;241";
    /// <summary>Color rgb(108, 108, 108)</summary>
    public static ReadOnlySpan<char> Color242 => "\x1b[38;5;242";
    /// <summary>Color rgb(118, 118, 118)</summary>
    public static ReadOnlySpan<char> Color243 => "\x1b[38;5;243";
    /// <summary>Color rgb(128, 128, 128)</summary>
    public static ReadOnlySpan<char> Color244 => "\x1b[38;5;244";
    /// <summary>Color rgb(138, 138, 138)</summary>
    public static ReadOnlySpan<char> Color245 => "\x1b[38;5;245";
    /// <summary>Color rgb(148, 148, 148)</summary>
    public static ReadOnlySpan<char> Color246 => "\x1b[38;5;246";
    /// <summary>Color rgb(158, 158, 158)</summary>
    public static ReadOnlySpan<char> Color247 => "\x1b[38;5;247";
    /// <summary>Color rgb(168, 168, 168)</summary>
    public static ReadOnlySpan<char> Color248 => "\x1b[38;5;248";
    /// <summary>Color rgb(178, 178, 178)</summary>
    public static ReadOnlySpan<char> Color249 => "\x1b[38;5;249";
    /// <summary>Color rgb(188, 188, 188)</summary>
    public static ReadOnlySpan<char> Color250 => "\x1b[38;5;250";
    /// <summary>Color rgb(198, 198, 198)</summary>
    public static ReadOnlySpan<char> Color251 => "\x1b[38;5;251";
    /// <summary>Color rgb(208, 208, 208)</summary>
    public static ReadOnlySpan<char> Color252 => "\x1b[38;5;252";
    /// <summary>Color rgb(218, 218, 218)</summary>
    public static ReadOnlySpan<char> Color253 => "\x1b[38;5;253";
    /// <summary>Color rgb(228, 228, 228)</summary>
    public static ReadOnlySpan<char> Color254 => "\x1b[38;5;254";
    /// <summary>Color rgb(238, 238, 238)</summary>
    public static ReadOnlySpan<char> Color255 => "\x1b[38;5;255";

    // Some named colors
    public static ReadOnlySpan<char> DarkRed => "\u001b[38;5;52m";
    public static ReadOnlySpan<char> DeepPink => "\u001b[38;5;53m";
    public static ReadOnlySpan<char> DarkOrange => "\u001b[38;5;130m";
    public static ReadOnlySpan<char> HotPink => "\u001b[38;5;162m";
    public static ReadOnlySpan<char> OrangeRed => "\u001b[38;5;202m";
    public static ReadOnlySpan<char> Gold => "\u001b[38;5;220m";
    public static ReadOnlySpan<char> LightGoldenrod => "\u001b[38;5;221m";
    public static ReadOnlySpan<char> OliveDrab => "\u001b[38;5;64m";
    public static ReadOnlySpan<char> YellowGreen => "\u001b[38;5;154m";
    public static ReadOnlySpan<char> LawnGreen => "\u001b[38;5;118m";
    public static ReadOnlySpan<char> GreenYellow => "\u001b[38;5;154m";
    public static ReadOnlySpan<char> DarkOliveGreen => "\u001b[38;5;58m";
    public static ReadOnlySpan<char> SpringGreen => "\u001b[38;5;48m";
    public static ReadOnlySpan<char> SeaGreen => "\u001b[38;5;83m";
    public static ReadOnlySpan<char> MediumAquamarine => "\u001b[38;5;79m";
    public static ReadOnlySpan<char> MediumSeaGreen => "\u001b[38;5;78m";
    public static ReadOnlySpan<char> LightSeaGreen => "\u001b[38;5;37m";
    public static ReadOnlySpan<char> DarkCyan => "\u001b[38;5;36m";
    public static ReadOnlySpan<char> LightCyan => "\u001b[38;5;195m";
    public static ReadOnlySpan<char> PaleTurquoise => "\u001b[38;5;159m";
    public static ReadOnlySpan<char> DeepSkyBlue => "\u001b[38;5;39m";
    public static ReadOnlySpan<char> DodgerBlue => "\u001b[38;5;33m";
    public static ReadOnlySpan<char> CornflowerBlue => "\u001b[38;5;69m";
    public static ReadOnlySpan<char> SteelBlue => "\u001b[38;5;67m";
    public static ReadOnlySpan<char> RoyalBlue => "\u001b[38;5;62m";
    public static ReadOnlySpan<char> MediumSlateBlue => "\u001b[38;5;99m";
    public static ReadOnlySpan<char> MediumPurple => "\u001b[38;5;98m";
    public static ReadOnlySpan<char> BlueViolet => "\u001b[38;5;57m";
    public static ReadOnlySpan<char> Indigo => "\u001b[38;5;54m";
    public static ReadOnlySpan<char> DarkOrchid => "\u001b[38;5;92m";
    public static ReadOnlySpan<char> DarkViolet => "\u001b[38;5;128m";
    public static ReadOnlySpan<char> MediumVioletRed => "\u001b[38;5;126m";
    public static ReadOnlySpan<char> PaleVioletRed => "\u001b[38;5;168m";
    public static ReadOnlySpan<char> LightPink => "\u001b[38;5;217m";
    public static ReadOnlySpan<char> LightCoral => "\u001b[38;5;210m";
    public static ReadOnlySpan<char> RosyBrown => "\u001b[38;5;138m";
    public static ReadOnlySpan<char> SandyBrown => "\u001b[38;5;215m";
    public static ReadOnlySpan<char> BurlyWood => "\u001b[38;5;180m";
    public static ReadOnlySpan<char> Tan => "\u001b[38;5;179m";
    public static ReadOnlySpan<char> Wheat => "\u001b[38;5;223m";
    public static ReadOnlySpan<char> NavajoWhite => "\u001b[38;5;222m";
    public static ReadOnlySpan<char> Bisque => "\u001b[38;5;224m";
    public static ReadOnlySpan<char> BlanchedAlmond => "\u001b[38;5;230m";
    public static ReadOnlySpan<char> Cornsilk => "\u001b[38;5;230m";
    public static ReadOnlySpan<char> OldLace => "\u001b[38;5;230m";
    public static ReadOnlySpan<char> FloralWhite => "\u001b[38;5;231m";
    public static ReadOnlySpan<char> Ivory => "\u001b[38;5;231m";
    public static ReadOnlySpan<char> AntiqueWhite => "\u001b[38;5;231m";
    public static ReadOnlySpan<char> Linen => "\u001b[38;5;231m";
    public static ReadOnlySpan<char> LavenderBlush => "\u001b[38;5;231m";
    public static ReadOnlySpan<char> MistyRose => "\u001b[38;5;224m";
    public static ReadOnlySpan<char> Gainsboro => "\u001b[38;5;246m";
    public static ReadOnlySpan<char> LightGray => "\u001b[38;5;250m";
    public static ReadOnlySpan<char> DarkGray => "\u001b[38;5;238m";
    public static ReadOnlySpan<char> DimGray => "\u001b[38;5;237m";
    public static ReadOnlySpan<char> SlateGray => "\u001b[38;5;241m";
    public static ReadOnlySpan<char> LightSlateGray => "\u001b[38;5;242m";

    // Shades of grey
    public static ReadOnlySpan<char> Grey0 => "\u001b[38;5;232m";
    public static ReadOnlySpan<char> Grey1 => "\u001b[38;5;233m";
    public static ReadOnlySpan<char> Grey2 => "\u001b[38;5;234m";
    public static ReadOnlySpan<char> Grey3 => "\u001b[38;5;235m";
    public static ReadOnlySpan<char> Grey4 => "\u001b[38;5;236m";
    public static ReadOnlySpan<char> Grey5 => "\u001b[38;5;237m";
    public static ReadOnlySpan<char> Grey6 => "\u001b[38;5;238m";
    public static ReadOnlySpan<char> Grey7 => "\u001b[38;5;239m";
    public static ReadOnlySpan<char> Grey8 => "\u001b[38;5;240m";
    public static ReadOnlySpan<char> Grey9 => "\u001b[38;5;241m";
    public static ReadOnlySpan<char> Grey10 => "\u001b[38;5;242m";
    public static ReadOnlySpan<char> Grey11 => "\u001b[38;5;243m";
    public static ReadOnlySpan<char> Grey12 => "\u001b[38;5;244m";
    public static ReadOnlySpan<char> Grey13 => "\u001b[38;5;245m";
    public static ReadOnlySpan<char> Grey14 => "\u001b[38;5;246m";
    public static ReadOnlySpan<char> Grey15 => "\u001b[38;5;247m";
    public static ReadOnlySpan<char> Grey16 => "\u001b[38;5;248m";
    public static ReadOnlySpan<char> Grey17 => "\u001b[38;5;249m";
    public static ReadOnlySpan<char> Grey18 => "\u001b[38;5;250m";
    public static ReadOnlySpan<char> Grey19 => "\u001b[38;5;251m";
    public static ReadOnlySpan<char> Grey20 => "\u001b[38;5;252m";
    public static ReadOnlySpan<char> Grey21 => "\u001b[38;5;253m";
    public static ReadOnlySpan<char> Grey22 => "\u001b[38;5;254m";
    public static ReadOnlySpan<char> Grey23 => "\u001b[38;5;255m";
}