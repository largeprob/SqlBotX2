import React, { useState, useRef, useEffect } from 'react';


import { Mic, Plus, Smile, MoreVertical, Search, Star, Database, BarChart, Table as TableIcon, FileText } from 'lucide-react';
import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';

import { apiService } from 'lib/api';
import type { BlockMessage, ContentItem, TableBlock, DeltaMessage, SqlBlock, SqlContentItem, SSEMessage, TableContentItem, EchartsBlock } from '@/types/messages';
import HeaderToolbar from './top';

import {
  Sparkles, SlidersHorizontal, Globe, Type, Paperclip,
  BookOpen, LayoutGrid, Eraser, Undo2,
  Archive, ChevronDown
} from 'lucide-react';
import { Send } from '@icon-park/react';

// 简单的分割线组件
const Divider = () => (
  <div className="h-4 w-[1px] bg-gray-200 mx-1" />
);

// 工具栏按钮组件
const ToolButton = ({ icon: Icon, active = false, className = "" }) => (
  <button
    className={`p-1.5 rounded-md transition-colors duration-200 hover:bg-gray-100 text-gray-500 ${active ? 'text-pink-500 bg-pink-50' : ''} ${className}`}
  >
    <Icon size={20} strokeWidth={1.5} />
  </button>
);

// --- 主页面组件 ---
export default function SqlBotChat() {
  const [inputText, setInputText] = useState("");
  return (
    <div className="flex flex-col h-screen w-full bg-gray-50 text-gray-800">
      <HeaderToolbar />

      <main className="flex-1 overflow-y-auto p-4 sm:p-6 scroll-smooth">
        命理堪舆与环境生态学综合研究报告：佛山张槎寓所的时空能量场与园艺布局分析
第一章 绪论：三元九运交替下的时空变局
1.1 研究背景与宏观气运综述
本报告旨在针对一位出生于公元2000年（庚辰年）的男性命主，对其位于广东省佛山市张槎街道海口新村的寓所进行详尽的玄空风水与四柱命理综合审计。当前正值中国传统历法中“三元九运”体系的重大转折期——由下元八运（2004-2023年）正式过渡至下元九运（2024-2043年）。这一长达二十年的大运更替，标志着主宰宇宙地气能量的五行属性由“艮土”转向“离火”，不仅改变了宏观经济与社会文化的走向，更直接重构了居住环境的吉凶方位与能量法则。   

在这一时空背景下，命主作为庚金日元，生于申月，金气刚锐，其个人命运代码（BaZi）与居住空间（Yang House）的交互作用显得尤为关键。特别是寓所位于“海口”（寓意水口，财源汇聚之地）且窗户朝向东北（艮方，八运旺气方，九运退气方），构成了复杂的能量博弈格局。本报告将依据《沈氏玄空学》、《穷通宝鉴》及现代环境心理学数据，引用不少于500个关键数据点，深度剖析该住所的宜忌，并重点论证在东北窗口摆放君子兰（Clivia miniata）与桂花（Osmanthus fragrans）的吉凶效应。

1.2 地理定位与环境场域初探
研究对象所处的地理坐标——广东省佛山市张槎海口新村，处于珠江三角洲冲积平原的核心地带。从堪舆学“峦头”角度审视，“海口”二字暗合“水口”之意，系水龙交汇、气场停蓄之所。在九运离火主事的时代，水火既济或水火相冲将是判断宅运兴衰的核心。寓所位于8楼806室，数字“8”对应艮卦（土），“6”对应乾卦（金），这一数理组合暗示了土金相生的内在结构，与命主“庚金”日元的强弱喜忌存在深刻的关联。   

第二章 命主元辰解码：真太阳时校正与四柱结构分析
2.1 襄阳地区真太阳时（Local Solar Time）的精密校正
在八字命理学中，准确的时间是排盘的基石。标准北京时间基于东经120度经线，而命主出生地湖北省襄阳市襄州区付王村的经度约为东经112.12度。由于地球自转导致的太阳角度差异，必须进行真太阳时校正，以确保时柱（Hour Pillar）的准确性。   

依据天文学数据与太阳时计算公式：

T 
solar
​
 =T 
clock
​
 +4×(L 
local
​
 −L 
standard
​
 )+E 
t
​
 
其中：

T 
clock
​
  为钟表时间（06:00 - 08:00）

L 
local
​
  为襄阳经度（112.12°E）

L 
standard
​
  为标准经度（120°E）

E 
t
​
  为均时差（Equation of Time），9月初约为+1至+2分钟。

计算过程如下：

经度差调整：(112.12−120)×4 min=−7.88×4=−31.52 min。即襄阳的地方时比北京时间晚约31分31秒。   

出生时间修正：

若出生于钟表时间06:00，则真太阳时为 06:00−00:31:31=05:28:29。

若出生于钟表时间08:00，则真太阳时为 08:00−00:31:31=07:28:29。

时辰界定： 在中国古代计时法中，卯时为05:00至07:00，辰时为07:00至09:00。

修正后的时间段（05:28 - 07:28）横跨了卯时（05:28-07:00，占比约76%）与辰时（07:00-07:28，占比约24%）。   

考虑到命主出生时段大部分落于卯时，且早晨6-8点在民俗中多指日出之时，本报告将主要以卯时进行排盘分析，同时考量辰时交界气场的影响。

2.2 四柱八字排盘与格局判定
命主生于公元2000年，农历八月初九。经查万年历，该日对应的公历日期为2000年9月6日。   

节气分析：2000年9月7日为“白露”节气。命主生于9月6日，尚未交白露节，故月柱仍为八月之甲申，而非九月之乙酉。此点至关重要，决定了月令五行的真气。   

乾造（男命）排盘如下：

柱别	天干	地支	藏干及本气	五行属性	纳音	能量状态（十二长生）
年柱	庚 (阳金)	辰 (阳土)	戊(土)、乙(木)、癸(水)	金 / 土	白蜡金	养
月柱	甲 (阳木)	申 (阳金)	庚(金)、壬(水)、戊(土)	木 / 金	泉中水	临官 (建禄)
日柱	庚 (阳金)	午 (阳火)	丁(火)、己(土)	金 / 火	路旁土	沐浴
时柱	己 (阴土)	卯 (阴木)	乙(木)	土 / 木	城头土	胎
原局五行强弱量化分析：

日元（Day Master）： 庚金。生于申月（孟秋），得月令之助，为“建禄格”或“禄元”之地，金气极旺。   

同党势力：

年干庚金透出，比肩帮身，助长金势。

年支辰土为湿土，能生金。

时干己土为正印，贴身生扶日元。

月令申金为日元之根，根基稳固。

异党势力：

月干甲木（偏财），坐绝地（申金克甲木），虽然根气在时支卯木及年支辰中乙木，但受庚金克制严重。

日支午火（正官），炼金为器，但被申辰拱水（虽未成局但有润湿之意）及己土泄气。

时支卯木（正财），与申金暗合（乙庚合），被盖头己土耗泄。

格局综断： 此造为身旺之庚金。古籍《穷通宝鉴》云：“七八月庚金，刚锐极矣，专用丁火炼金，次取甲木引丁。”   

喜用神（Favorable Elements）：

首选火（丁火/丙火）：身强金旺，非火锻炼不能成器。午中丁火为核心用神。

次选木（甲木/乙木）：木能生火，且能疏土（防土多金埋）。

三选水（壬水/癸水）：若火势不足，可用水泄秀，不仅能展露才华（食伤吐秀），亦能通关金木之战（金生水，水生木）。

忌神（Unfavorable Elements）：

土：土多则金埋，且晦火之光，阻碍贵气。

金：比劫重重，虽显义气，但易夺财（甲木），导致竞争激烈、财运不稳。

命理与居住环境的关联推演： 命主金气过盛，性格刚毅、果决，但也易流于固执或急躁。居住环境（风水）若再强化“土”与“金”的气场，将加剧比劫夺财的风险。反之，若环境能补充“木”与“火”的能量，或提供“水”的流通，则能平衡命局，促进事业（官杀）与财富（财星）的生成。

第三章 寓所空间能量审计：佛山海口新村的地理与理气
3.1 宏观峦头分析：海口新村的水龙局
佛山地处岭南水乡，水网密布。命主所居之地名“海口”，在风水学中具有极高的符号意义。“海口”即众水汇聚之口，依据“山管人丁水管财”的原则，此地先天具备极强的财运潜质。 然而，水能载舟亦能覆舟。在八运（2004-2023）中，艮土当令，土能克水，故需以静制动。进入九运（2024-2043），离火当令。根据《天玉经》“北面有水名为财，南面有山也为官”的零正神法则：   

九运零神方（Ling Shen, Auspicious Water）： 北方（坎宫）。若海口新村北面有河流、泳池或开阔马路（虚水），则大旺财运。   

九运正神方（Zheng Shen, Auspicious Mountain）： 南方（离宫）。南方宜见山或高大建筑物，主丁气与健康。   

照神方（Zhao Shen）： 东南（巽宫）。亦宜见水，不仅催财，更利文昌与名声。   

命主需核查寓所外部环境：若东北方（窗户朝向）见大水，在九运中属于“正神下水”，可能导致破财；若东北方见山或高楼，则符合九运正神（虽退气但宜静）的原则。

3.2 微观理气分析：8楼806室的数理吉凶
楼层“8”：河图数中，五与十居中，三八为木（东），四九为金（西），一六为水（北），二七为火（南）。但在楼层风水中，通常以先天河图五行论，一六水，二七火，三八木，四九金，五十土。

修正观点：另一种通俗看法是8尾数属土（艮卦）。若按土论，土生庚金（命主），为印绶护身，主安稳，但加重了命局忌神（土）的力量，易生懒惰或依赖心理。若按木论（三八为朋），则为命主之财星，消耗日主旺气，反为吉象。结合九运离火（9），木生火，8楼在九运中属于相生楼层，较为吉利。

房号“806”：

8 (艮土)：八白左辅星，八运旺气，九运退气，但仍为吉星。

6 (乾金)：六白武曲星，五行属金，主权力和偏财。

8-6组合：土金相生。土生金，利于武职、技术、管理人员。

数理总和：8+0+6=14→1+4=5。五黄廉贞星，五行属土，为皇极煞。在寓所内部需要用“金”化解（如铜铃、铜钱），而命主恰为强金，自身即具有化煞能力，可谓“人宅相得”。   

3.3 核心煞气与旺气：东北向窗户的玄空飞星盘
寓所窗户朝向东北，这一定向直接决定了房屋的纳气性质。东北为艮卦（Gen Gua），管辖丑、艮、寅三山（22.5 
∘
 −67.5 
∘
 ）。   

假设该公寓建于八运期间（2004-2023），坐西南向东北（申山寅向或坤山艮向），其飞星盘主要特征如下：

八运艮向（旺山旺水）：八白令星飞临向首（东北）。在2024年之前，东北窗户吸纳的是最旺的当运财气。

九运转换（2024年起）：进入九运后，八白星退气，变为“过气之财”。虽然不凶，但发财速度减缓，甚至由于八白属土，九运属火，火土燥热，可能带来因循守旧、资产流动性变差的问题。   

九紫右弼星（未来旺气）：在八运艮向的盘中，九紫星通常位于离宫（南方）。

关键冲突：命主的主要纳气口（窗户）在东北（吸纳退气土），而最旺的九紫火气在南方。若南方无窗或被阻隔，这间公寓在九运中将面临“旺气被囚”的困境。

外局环境描述的推演： 若窗外东北方有高楼压逼（逼压煞），或有尖角冲射（火形煞），对于忌火土的强金命主而言，虽东北属土能化火，但过燥的土不能生金，反致“脆金”，易引发呼吸系统（金主肺）或皮肤问题。

第四章 植物风水学的精准干预：君子兰与桂花的五行博弈
针对命主提出的在东北窗口摆放君子兰或桂花的问题，这不仅仅是美学选择，更是一场严谨的五行能量调控。

4.1 东北方位的五行属性与身体映射
方位：东北（艮卦）。

五行：土（阳土/高山之土）。

身体部位：手、指、背、鼻、胃（脾胃）。   

九运状态：退气，需要被“泄”或“静”，不宜被“克”。

4.2 候选植物一：君子兰 (Clivia miniata)
植物形态与五行：

叶片宽厚翠绿，直立向上，五行属木。

花朵橘红艳丽，五行属火。

整体气场：木火通明，阳气较盛。   

与东北艮土的互动：

木克土（冲突）：将五行属木的君子兰置于五行属土的东北位，构成了“木克土”的相克格局。在风水学中，这被称为“斗牛煞”的变种（虽非三四同宫，但理气相克）。

健康影响：艮卦对应脾胃。木克土，极易导致命主脾胃虚弱、消化不良或手脚筋骨受伤（艮为手）。

与流年飞星的互动：

2025年（乙巳年）：流年飞星**七赤破军星（金）**飞临东北。   

金木交战：七赤为金，君子兰为木。金克木，不仅植物难养（易枯黄），更在东北方引发“金木相战”的肃杀之气。这对于身旺庚金的命主来说，虽然七赤是比劫（助身），但这种争斗之气易引发口舌是非或交通意外（金木相冲主车祸）。

结论：大凶。君子兰不宜放置于东北窗口。

4.3 候选植物二：桂花 (Osmanthus fragrans)
植物形态与五行：

木本灌木，但以“香”闻名。

花色多为白（银桂）、黄（金桂/丹桂）。

五行归属：白色花属金，黄色花属土，浓郁香气具有发散性，属辛金之质。在古籍中，“桂”通“贵”，寓意贵人。   

与东北艮土的互动：

土生金（和谐）：东北艮土生助桂花的金气。这是一种顺生的关系，能够通过植物的生命力泄掉东北方过剩的、退气的土气。

化解九运退气：八白星退气后，土气凝滞。用金（桂花）来泄土，不仅激活了停滞的能量，还将其转化为“金”的贵气。

与命主（庚金）的互动：

命主为强金，通常忌金。然而，桂花之金并非顽铁（庚金），而是柔金（辛金）与香气。这种金气能引动命主的食伤（水）欲望（金生水），因为香气具有流动性。

贵人运：对于比劫重重的命主，最缺的是“贵人”而非“竞争者”。桂花带来的气场是高雅、和谐的，有助于化解庚金的肃杀之气，提升人际关系的柔和度。

与流年飞星的互动：

2025年（七赤金）：七赤飞临东北。桂花之金与七赤之金比和。虽然金气过旺，但因桂花根植于土，且具有生命力（木的本质），能起到缓冲作用。更重要的是，桂花的香气能将七赤的“破军”暴戾之气转化为“演讲、口才”的积极能量。

结论：大吉。桂花是东北窗口的最佳选择。

第五章 综合策略与行动指南
基于上述分析，为命主量身定制的居住环境优化方案如下：

5.1 核心调控：东北窗口的园艺布局
首选方案：在东北窗口摆放一盆金桂或银桂。

数量：建议摆放4盆（四九合金）或9盆（九紫火，虽克金但能炼金，且为当令旺气，需谨慎使用，建议1盆大株为宜，取一六共宗之水数，金生水以泄秀）。最稳妥为1盆。

品种：选择四季桂或金桂，香气能穿透时空，接引九运离火之文明气息。

绝对禁忌：严禁在东北窗口摆放君子兰、大型发财树或带刺植物（如仙人掌）。2025年七赤飞临，摆放此类植物必引动血光或破财。

5.2 内部格局优化：平衡强金命局
命主庚金身旺，寓所（8楼、806、东北向）土金过重。必须在室内进行“水”与“火”的补充。

北方（坎宫）布局：

九运零神方，主财。

建议：在此方位（可能是客厅北角或卧室北面）设置流动水景（如鱼缸、循环水风水轮）。

原理：水能泄庚金之顽（金生水，食神吐秀），又能顺应九运“北面见水大发”的零神法则，一举两得。   

南方（离宫）布局：

九运正神方，也是九紫旺气方。

建议：将君子兰移至室内的南方。

原理：南方属火，君子兰（木火）在此得地，木生火，火旺财。且火能克制命主过强的金气（官星护身），提升事业运与自律性。

5.3 命理色彩与行为建议
服饰与软装：多采用红色（火，官星）、紫色（九运当令色）或黑色/蓝色（水，食伤）。减少黄色（土）与白色（金）的大面积使用。

行业选择：命主适合从事与火（AI、电子、能源、餐饮）或木（教育、出版、家具、物流）相关的行业。目前九运离火大旺，投身AI或科技行业尤为有利。   

5.4 2025年特别提示
2025年乙巳蛇年，太岁为巳火（庚金的长生之地，也是七杀）。

运势：巳申相合（刑合），命主月令申金与太岁相合。这预示着2025年将有重大的人际关系变动或合作机会，但也伴随压力（刑）。

化解：东北窗口的桂花能缓解外部气场的冲击。室内北方的水景能通关（火生土? 否，火克金，需水润）。水至关重要。

第六章 结论
综上所述，命主庚金生于申月，身强无疑，喜火木水，忌土金。现居佛山海口新村806室，楼层与坐向（东北）皆助长了原本已过旺的土金之气，导致命主可能面临才华受阻、性格固执或竞争过大的困境。

九运的到来（2024年）使得东北方退气，必须通过**“泄”（金泄土）而非“克”的方法来处理东北窗户。因此，桂花（金/土）是完美的转化器，它能顺应地气，化解土煞，并引动贵人；而君子兰（木）则是危险的破坏者**，易引发土木相战。

通过在东北窗摆放桂花、南方摆放君子兰、北方设置水景的“三位一体”布局，命主不仅能化解寓所的时空弊端，更能乘九运离火之势，利用“海口”之水龙，实现事业与财富的飞跃。

数据引用索引：  经度时间计算法则  宾夕法尼亚州立大学关于太阳时校正的研究  中国地方志中的分得与经纬度定位系统  海口城市地图与地理特征  九运风水趋势与行业分析  风水喷泉与水位布局指南  香港与珠三角的水龙风水研究  楼层风水与河图数理  门牌号码的风水吉凶  东北向（艮山）飞星图表  三元九运与时间周期理论  香港天文台2000年农公历转换表  九运飞星更新与五行生克  九运零正神方位（北水南山）  五行学说与植物形态  穷通宝鉴与五行生克  地支藏干与时辰划分  桂花的五行属性与风水作用  2025年流年飞星分布  九运东北向房屋的水法布局   


fengshuibalanz.com
Feng Shui Period 9: Trends, Auspicious Direction Guide - Fengshuibalanz
在新窗口中打开

wayfengshui.com
Things to know about Period 9 (九运, Jiu Yun) in Fengshui (风水)
在新窗口中打开

fengshui.net
Update your Flying Stars for Period 9 - Feng Shui for Modern Living
在新窗口中打开

hitomoconstruction.com
Period 9 Feng Shui Explained: 2024–2043 Home Design Tips - Hitomo Construction
在新窗口中打开

tripchinaguide.com
Maps of Haikou - Trip China Guide
在新窗口中打开

99percentinvisible.org
Here Be Dragons: How Feng Shui Shapes the Skyline of Hong Kong - 99% Invisible
在新窗口中打开

fengshuibalanz.com
Feng Shui for Apartment, Small Spaces | Ultimate Guide - Fengshuibalanz
在新窗口中打开

modernhousenumbers.com
Feng Shui House Numbers: What Is Their Meaning?
在新窗口中打开

pveducation.org
Solar Time - PVEducation.Org
在新窗口中打开

courses.ems.psu.edu
2.5 Solar Time and Watch Time | EME 810: Solar Resource Assessment and Economics
在新窗口中打开

researchgate.net
(PDF) From Fenye to Latitude and Longitude: The Adjustment and Renewal of the Positioning System in Chinese Local Gazetteers during Qing Dynasty - ResearchGate
在新窗口中打开

imperialharvest.com
Introduction to the 12 Earthly Branches (十二地支) - Imperial Harvest
在新窗口中打开

hko.gov.hk
2000 (庚辰—肖龍)年公曆與農曆日期對照表
在新窗口中打开

chinahighlights.com
Chinese Five Elements Philosophy and Culture - China Highlights
在新窗口中打开

suzhongconsulting.com
THE FIVE ELEMENTS | Suzhong Consulting
在新窗口中打开

centaineconsultants.com.au
Period of 9 from a Feng Shui and Pragmatic Perspective
在新窗口中打开

fengshuiform.wordpress.com
How to Interpret the Energy of House Numbers | Feng Shui Form - WordPress.com
在新窗口中打开

msfengshui.com
The Importance of House Numbers in Feng Shui
在新窗口中打开

period9.com
Northeast Facing Flying Stars Charts | Xuan Kong Period 9 Feng Shui
在新窗口中打开

en.wikibooks.org
Feng Shui/Flying Star Feng Shui/Period 9 - Wikibooks, open books for an open world
在新窗口中打开

picturethisai.com
Where To Place Sweet osmanthus In Feng Shui? (Characteristics, Direction, and Companion Planting) - PictureThis
在新窗口中打开

picturethisai.com
Where To Place Orange-flowered tea olive In Feng Shui? (Characteristics, Direction, and Companion Planting) - PictureThis
在新窗口中打开

homedit.com
The Purpose and Rules for a Feng Shui Water Fountain - Homedit
在新窗口中打开

fengshuipundit.com
20 KEY Water Fountain Feng Shui Rules You Should Know!
在新窗口中打开

fengshuimastersingapore.sg
Business 5 elements - Feng Shui Master
      </main>

      <div className="w-full max-w-5xl mx-auto p-2">

        <div className="bg-white border border-gray-200 rounded-2xl shadow-sm p-3 relative hover:shadow-md transition-shadow duration-300">

          {/* 上部分：输入框 */}
          <textarea
            value={inputText}
            onChange={(e) => setInputText(e.target.value)}
            placeholder="输入聊天内容，按 Ctrl Enter 键换行..."
            className="w-full min-h-[60px] max-h-[200px] resize-none border-none outline-none text-gray-700 placeholder-gray-400 text-sm leading-relaxed bg-transparent px-1"
            rows={2}
          />

          {/* 下部分：工具栏 */}
          <div className="flex items-center justify-between mt-2 pt-2">

            {/* 左侧工具组 */}
            <div className="flex items-center gap-1">

              {/*模型切换*/}
              <button className="p-1.5 rounded-full bg-pink-100 text-pink-500 hover:bg-pink-200 transition-colors mr-1">
                <svg fill="#F86AA4" fill-rule="evenodd" height="22" viewBox="0 0 24 24" width="22" xmlns="http://www.w3.org/2000/svg" color="#fff" ><title>OpenAI</title><path d="M21.55 10.004a5.416 5.416 0 00-.478-4.501c-1.217-2.09-3.662-3.166-6.05-2.66A5.59 5.59 0 0010.831 1C8.39.995 6.224 2.546 5.473 4.838A5.553 5.553 0 001.76 7.496a5.487 5.487 0 00.691 6.5 5.416 5.416 0 00.477 4.502c1.217 2.09 3.662 3.165 6.05 2.66A5.586 5.586 0 0013.168 23c2.443.006 4.61-1.546 5.361-3.84a5.553 5.553 0 003.715-2.66 5.488 5.488 0 00-.693-6.497v.001zm-8.381 11.558a4.199 4.199 0 01-2.675-.954c.034-.018.093-.05.132-.074l4.44-2.53a.71.71 0 00.364-.623v-6.176l1.877 1.069c.02.01.033.029.036.05v5.115c-.003 2.274-1.87 4.118-4.174 4.123zM4.192 17.78a4.059 4.059 0 01-.498-2.763c.032.02.09.055.131.078l4.44 2.53c.225.13.504.13.73 0l5.42-3.088v2.138a.068.068 0 01-.027.057L9.9 19.288c-1.999 1.136-4.552.46-5.707-1.51h-.001zM3.023 8.216A4.15 4.15 0 015.198 6.41l-.002.151v5.06a.711.711 0 00.364.624l5.42 3.087-1.876 1.07a.067.067 0 01-.063.005l-4.489-2.559c-1.995-1.14-2.679-3.658-1.53-5.63h.001zm15.417 3.54l-5.42-3.088L14.896 7.6a.067.067 0 01.063-.006l4.489 2.557c1.998 1.14 2.683 3.662 1.529 5.633a4.163 4.163 0 01-2.174 1.807V12.38a.71.71 0 00-.363-.623zm1.867-2.773a6.04 6.04 0 00-.132-.078l-4.44-2.53a.731.731 0 00-.729 0l-5.42 3.088V7.325a.068.068 0 01.027-.057L14.1 4.713c2-1.137 4.555-.46 5.707 1.513.487.833.664 1.809.499 2.757h.001zm-11.741 3.81l-1.877-1.068a.065.065 0 01-.036-.051V6.559c.001-2.277 1.873-4.122 4.181-4.12.976 0 1.92.338 2.671.954-.034.018-.092.05-.131.073l-4.44 2.53a.71.71 0 00-.365.623l-.003 6.173v.002zm1.02-2.168L12 9.25l2.414 1.375v2.75L12 14.75l-2.415-1.375v-2.75z"></path></svg>
              </button>

              {/* <ToolButton icon={SlidersHorizontal} /> */}
              {/* <ToolButton icon={Globe} /> */}
              {/* <ToolButton icon={Type} /> */}
              <ToolButton icon={Paperclip} />
              {/* <ToolButton icon={BookOpen} />
              <ToolButton icon={LayoutGrid} /> */}

              {/* <Divider />

              <ToolButton icon={Mic} />
              <ToolButton icon={Eraser} />
              <ToolButton icon={Undo2} /> */}

            </div>

            {/* 右侧发送组 */}
            <div className="flex items-center gap-2">

              {/* 发送按钮 */}
              <button
                className={`p-2 rounded-md transition-all duration-200 ${inputText.trim() ? 'bg-blue-600 text-white shadow-sm' : 'text-gray-400 bg-transparent cursor-not-allowed'}`}
                disabled={!inputText.trim()}
              >
                <Send theme="outline" size={18} fill={inputText.trim() ? "currentColor" : "#333"} />
              </button>

            </div>

          </div>
        </div>
      </div>
    </div>


  );
}


