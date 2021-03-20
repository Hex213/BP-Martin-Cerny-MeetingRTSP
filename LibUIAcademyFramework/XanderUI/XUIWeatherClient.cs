using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;

namespace LibUIAcademyFramework.XanderUI
{
    public class XUIWeatherClient : Component
    {
        // Fields
        private BackgroundWorker backgroundThread = new BackgroundWorker();
        private string CitiesPath = "albania/tirana, algeria/algiers, angola/luanda, antigua-and-barbuda/saint-johns, argentina/buenos-aires, argentina/cordoba, armenia/yerevan, australia/canberra, australia/sydney, australia/alice-springs, australia/darwin, australia/brisbane, australia/cairns, australia/adelaide, australia/hobart, australia/melbourne, australia/eucla, australia/perth, austria/vienna, azerbaijan/baku, bahamas/nassau, bahrain/manama, bangladesh/dhaka, barbados/bridgetown, belarus/minsk, belgium/brussels, belize/belmopan, benin/porto-novo, bermuda/hamilton, bhutan/thimphu, bolivia/la-paz, bosnia-herzegovina/sarajevo, botswana/gaborone, brazil/rio-branco, brazil/manaus, brazil/salvador, brazil/fortaleza, brazil/brasilia, brazil/belem, brazil/recife, brazil/rio-de-janeiro, brazil/sao-paulo, brunei/bandar-seri-begawan, bulgaria/sofia, burundi/bujumbura, cape-verde/praia, cambodia/phnom-penh, cameroon/yaounde, canada/calgary, canada/edmonton, canada/vancouver, canada/winnipeg, canada/saint-john, canada/happy-valley-goose-bay, canada/mary-s-harbour, canada/st-johns, canada/inuvik, canada/yellowknife, canada/halifax, canada/baker-lake, canada/coral-harbour, canada/eureka, canada/grise-fiord, canada/pond-inlet, canada/resolute, canada/ottawa, canada/toronto, canada/blanc-sablon, canada/chibougamau, canada/kuujjuaq, canada/montreal, canada/quebec, canada/regina, canada/whitehorse, uk/georgetown, central-african-republic/bangui, chad/ndjamena, chile/easter-island, chile/punta-arenas, chile/santiago, china/beijing, china/chongqing, china/shenzhen, china/suzhou, china/shanghai, china/lhasa, china/urumqi, colombia/bogota, comoros/moroni, congo/brazzaville, congo-demrep/kinshasa, cook-islands/rarotonga, cote-divoire/abidjan, cote-divoire/yamoussoukro, croatia/zagreb, cuba/havana, cyprus/nicosia, czech-republic/prague, denmark/copenhagen, djibouti/djibouti, dominica/roseau, dominican-republic/santo-domingo, ecuador/guayaquil, ecuador/quito, egypt/cairo, el-salvador/san-salvador, equatorial-guinea/malabo, estonia/tallinn, ethiopia/addis-ababa, falkland/stanley, faroe/torshavn, fiji/suva, finland/helsinki, finland/kemi, finland/rovaniemi, france/paris, france/cayenne, france/papeete, france/martin-de-vivies-amsterdam-island, kergulen/port-aux-francais, gabon/libreville, gambia/banjul, georgia/tbilisi, germany/berlin, germany/frankfurt, ghana/accra, gibraltar/gibraltar, greece/athens, greenland/danmarkshavn, greenland/kangerlussuaq, greenland/nuuk, greenland/thule-air-base, grenada/saint-georges, guadeloupe/basse-terre, usa/guam-hagatna, guatemala/guatemala, guinea-bissau/bissau, haiti/port-au-prince, honduras/tegucigalpa, hong-kong/hong-kong, hungary/budapest, iceland/reykjavik, india/patna, india/delhi, india/new-delhi, india/bangalore, india/thiruvananthapuram, india/indore, india/mumbai, india/pune, india/bhubaneshwar, india/ahmedgarh, india/ludhiana, india/chennai, india/varanasi, india/kolkata, indonesia/denpasar, indonesia/surabaya, indonesia/balikpapan, indonesia/jakarta, indonesia/manado, indonesia/jayapura, indonesia/makassar, indonesia/bandung, indonesia/pontianak, indonesia/manokwari, iran/tehran, iraq/baghdad, ireland/dublin, isle-of-man/douglas, israel/jerusalem, israel/tel-aviv, italy/milan, italy/rome, jamaica/kingston, japan/kobe, japan/kyoto, japan/nagoya, japan/osaka, japan/sapporo, japan/tokyo, japan/yokohama, jordan/amman, kazakstan/almaty, kazakstan/aqtobe, kazakstan/astana, kazakhstan/oral, kenya/nairobi, kiribati/kiritimati, kosovo/pristina, kuwait/kuwait-city, kyrgyzstan/bishkek, laos/vientiane, latvia/riga, lebanon/beirut, liechtenstein/vaduz, lithuania/vilnius, luxembourg/luxembourg, republic-of-macedonia/skopje, madagascar/antananarivo, malaysia/kuala-lumpur, maldives/male, mali/bamako, malta/valletta, marshall-islands/majuro, martinique/fort-de-france, mauritania/nouakchott, mauritius/port-louis, mexico/aguascalientes, mexico/mexicali, mexico/tijuana, mexico/mexico-city, mexico/acapulco, mexico/guadalajara, mexico/cancun, mexico/mazatlan, mexico/hermosillo, mexico/veracruz, micronesia/palikir, moldova/chisinau, monaco/monaco, mongolia/hovd, mongolia/ulaanbaatar, montenegro/podgorica, morocco/casablanca, morocco/rabat, mozambique/maputo, myanmar/yangon, namibia/windhoek, nepal/kathmandu, netherlands/amsterdam, new-zealand/auckland, new-zealand/wellington, nicaragua/managua, niger/niamey, nigeria/lagos, niue/alofi, north-korea/pyongyang, norway/longyearbyen, norway/oslo, norway/tromso, oman/muscat, pakistan/islamabad, pakistan/karachi, pakistan/lahore, palau/ngerulmud, panama/panama, papua-new-guinea/port-moresby, paraguay/asuncion, peru/lima, philippines/manila, poland/warsaw, portugal/lisbon, portugal/ponta-delgada-azores, puerto-rico/san-juan, qatar/doha, reunion/saint-denis, romania/bucharest, russia/belushya-guba, russia/ufa, russia/chelyabinsk, russia/anadyr, russia/pevek, russia/irkutsk, russia/kaliningrad, russia/petropavlovsk-kamchatsky, russia/komsomolsk-on-amur, russia/khatanga, russia/krasnoyarsk, russia/norilsk, russia/magadan, russia/moscow, russia/murmansk, russia/novgorod, russia/novosibirsk, russia/omsk, russia/perm, russia/vladivostok, russia/saint-peterburg, russia/tiksi, russia/verkhoyansk, russia/yakutsk, russia/yuzhno-sakhalinsk, russia/samara, russia/yekaterinburg, russia/kazan, russia/izhevsk, russia/chita, rwanda/kigali, uk/jamestown, saint-kitts-and-nevis/basseterre, saint-lucia/castries, saint-vincent-and-grenadines/kingstown, samoa/apia, san-marino/san-marino, sao-tome-and-principe/sao-tome, saudi-arabia/makkah, saudi-arabia/medina, saudi-arabia/riyadh, senegal/dakar, serbia/belgrade, seychelles/victoria, singapore/singapore, slovakia/bratislava, slovenia/ljubljana, solomon-islands/honiara, south-africa/cape-town, south-africa/johannesburg, south-africa/marion-island-prince-edward-islands, south-africa/pretoria, south-korea/seoul, spain/barcelona, spain/madrid, spain/palma, sri-lanka/colombo, sri-lanka/sri-jayawardenapura-kotte, sudan/khartoum, suriname/paramaribo, sweden/stockholm, switzerland/bern, switzerland/geneva, switzerland/zurich, syria/damascus, taiwan/taipei, tajikistan/dushanbe, tanzania/dar-es-salaam, tanzania/dodoma, thailand/bangkok, togo/lome, tonga/nukualofa, trinidad-and-tobago/port-of-spain, tunisia/tunis, turkey/ankara, turkey/istanbul, turkmenistan/ashgabat, tuvalu/funafuti, uganda/kampala, ukraine/dnipro, ukraine/kyiv, ukraine/odesa, united-arab-emirates/abu-dhabi, united-arab-emirates/dubai, uk/london, uk/belfast, uk/edinburgh, uk/glasgow, uk/cardiff, uruguay/montevideo, usa/midway, usa/wake-island, usa/montgomery, usa/adak, usa/anchorage, usa/fairbanks, usa/juneau, usa/unalaska, usa/phoenix, usa/little-rock, usa/los-angeles, usa/sacramento, usa/san-diego, usa/san-francisco, usa/san-jose, usa/denver, usa/hartford, usa/dover, usa/washington-dc, usa/miami, usa/orlando, usa/pensacola, usa/atlanta, usa/honolulu, usa/boise, usa/chicago, usa/indianapolis, usa/des-moines, usa/topeka, usa/louisville, usa/new-orleans, usa/augusta, usa/baltimore, usa/boston, usa/detroit, usa/minneapolis, usa/st-paul, usa/jackson, usa/kansas-city, usa/st-louis, usa/billings, usa/lincoln, usa/las-vegas, usa/concord, usa/newark, usa/albuquerque, usa/new-york, usa/raleigh, usa/bismarck, usa/columbus, usa/oklahoma-city, usa/portland-or, usa/salem, usa/philadelphia, usa/providence, usa/columbia, usa/rapid-city, usa/sioux-falls, usa/knoxville, usa/nashville, usa/austin, usa/dallas, usa/houston, usa/midland, usa/salt-lake-city, usa/montpelier, usa/richmond, usa/seattle, usa/charleston-wv, usa/madison, usa/milwaukee, usa/cheyenne, uzbekistan/tashkent, vanuatu/port-vila, vatican-city-state/vatican-city, venezuela/caracas, vietnam/hanoi, vietnam/ho-chi-minh, western-sahara/el-aaiun, zambia/lusaka, zimbabwe/harare ";
        private string degrees;
        private string currentTime;
        private string forecast;
        private string wind;
        private string description;
        private string weatherImage;
        private Cities city;

        // Methods
        public XUIWeatherClient()
        {
            this.backgroundThread.DoWork += new DoWorkEventHandler(this.backgroundThread_DoWork);
        }

        private void backgroundThread_DoWork(object sender, DoWorkEventArgs e)
        {
            string str = this.city.ToString().Replace("_", "-").ToLower();
            char[] separator = new char[] { ',' };
            foreach (string str2 in this.CitiesPath.Replace(" ", "").Split(separator))
            {
                char[] chArray2 = new char[] { '/' };
                if (str2.Split(chArray2).Last<string>() == str)
                {
                    string str3;
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                    WebClient client1 = new WebClient();
                    client1.Proxy = null;
                    using (StreamReader reader = new StreamReader(client1.OpenRead("https://www.timeanddate.com/weather/" + str2)))
                    {
                        str3 = reader.ReadToEnd();
                    }
                    char[] chArray3 = new char[] { '\n' };
                    foreach (string str4 in str3.Split(chArray3))
                    {
                        if (str4.StartsWith("<header class=fixed><h1>"))
                        {
                            char[] chArray4 = new char[] { '&' };
                            char[] chArray5 = new char[] { '>' };
                            this.degrees = str4.Split(chArray4)[1].Split(chArray5).Last<string>();
                            char[] chArray6 = new char[] { '<' };
                            string[] strArray3 = str4.Split(chArray6);
                            int index = 0;
                            while (true)
                            {
                                if (index >= strArray3.Length)
                                {
                                    char[] chArray8 = new char[] { ':' };
                                    char[] chArray9 = new char[] { '&' };
                                    this.forecast = str4.Split(chArray8)[2].Split(chArray9).First<string>();
                                    char[] chArray10 = new char[] { ':' };
                                    char[] chArray11 = new char[] { '<' };
                                    this.wind = str4.Split(chArray10)[3].Split(chArray11).First<string>();
                                    char[] chArray12 = new char[] { '<' };
                                    strArray3 = str4.Split(chArray12);
                                    index = 0;
                                    while (index < strArray3.Length)
                                    {
                                        string str6 = strArray3[index];
                                        if (str6.Contains("cur-weather"))
                                        {
                                            char[] chArray13 = new char[] { '"' };
                                            this.description = str6.Split(chArray13)[1];
                                            char[] chArray14 = new char[] { '"' };
                                            this.weatherImage = str6.Split(chArray14)[3].Replace("//", "");
                                        }
                                        index++;
                                    }
                                    break;
                                }
                                string str5 = strArray3[index];
                                if (str5.Contains("id=wtct"))
                                {
                                    char[] chArray7 = new char[] { '>' };
                                    this.currentTime = str5.Split(chArray7).Last<string>();
                                }
                                index++;
                            }
                        }
                    }
                }
            }
        }

        public void syncWeather(Cities c)
        {
            this.city = c;
            this.backgroundThread.RunWorkerAsync();
        }

        // Properties
        [Category("XanderUI"), Browsable(true), Description("The degrees of the location in celsius")]
        public string Degrees =>
            this.degrees;

        [Category("XanderUI"), Browsable(true), Description("The current time of the location")]
        public string CurrentTime =>
            this.currentTime;

        [Category("XanderUI"), Browsable(true), Description("The forecast of the location")]
        public string Forecast =>
            this.forecast;

        [Category("XanderUI"), Browsable(true), Description("The wind speeed of the location in celsius")]
        public string Wind =>
            this.wind;

        [Category("XanderUI"), Browsable(true), Description("The description of the wether")]
        public string Description =>
            this.description;

        [Category("XanderUI"), Browsable(true), Description("Returns an image of the current weather")]
        public string WeatherImage =>
            this.weatherImage;

        // Nested Types
        public enum Cities
        {
            Tirana,
            Algiers,
            Luanda,
            Saint_johns,
            Buenos_aires,
            Cordoba,
            Yerevan,
            Canberra,
            Sydney,
            Alice_springs,
            Darwin,
            Brisbane,
            Cairns,
            Adelaide,
            Hobart,
            Melbourne,
            Eucla,
            Perth,
            Vienna,
            Baku,
            Nassau,
            Manama,
            Dhaka,
            Bridgetown,
            Minsk,
            Brussels,
            Belmopan,
            Porto_novo,
            Hamilton,
            Thimphu,
            La_paz,
            Sarajevo,
            Gaborone,
            Rio_branco,
            Manaus,
            Salvador,
            Fortaleza,
            Brasilia,
            Belem,
            Recife,
            Rio_de_janeiro,
            Sao_paulo,
            Bandar_seri_begawan,
            Sofia,
            Bujumbura,
            Praia,
            Phnom_penh,
            Yaounde,
            Calgary,
            Edmonton,
            Vancouver,
            Winnipeg,
            Saint_john,
            Happy_valley_goose_bay,
            Mary_s_harbour,
            St_johns,
            Inuvik,
            Yellowknife,
            Halifax,
            Baker_lake,
            Coral_harbour,
            Eureka,
            Grise_fiord,
            Pond_inlet,
            Resolute,
            Ottawa,
            Toronto,
            Blanc_sablon,
            Chibougamau,
            Kuujjuaq,
            Montreal,
            Quebec,
            Regina,
            Whitehorse,
            Georgetown,
            Bangui,
            Ndjamena,
            Easter_island,
            Punta_arenas,
            Santiago,
            Beijing,
            Chongqing,
            Shenzhen,
            Suzhou,
            Shanghai,
            Lhasa,
            Urumqi,
            Bogota,
            Moroni,
            Brazzaville,
            Kinshasa,
            Rarotonga,
            San_jose,
            Abidjan,
            Yamoussoukro,
            Zagreb,
            Havana,
            Nicosia,
            Prague,
            Copenhagen,
            Djibouti,
            Roseau,
            Santo_domingo,
            Guayaquil,
            Quito,
            Cairo,
            San_salvador,
            Malabo,
            Tallinn,
            Addis_ababa,
            Stanley,
            Torshavn,
            Suva,
            Helsinki,
            Kemi,
            Rovaniemi,
            Paris,
            Cayenne,
            Papeete,
            Martin_de_vivies_amsterdam_island,
            Port_aux_francais,
            Libreville,
            Banjul,
            Tbilisi,
            Berlin,
            Frankfurt,
            Accra,
            Gibraltar,
            Athens,
            Danmarkshavn,
            Kangerlussuaq,
            Nuuk,
            Thule_air_base,
            Saint_georges,
            Basse_terre,
            Guam_hagatna,
            Guatemala,
            Bissau,
            Port_au_prince,
            Tegucigalpa,
            Hong_kong,
            Budapest,
            Reykjavik,
            Patna,
            Delhi,
            New_delhi,
            Bangalore,
            Thiruvananthapuram,
            Indore,
            Mumbai,
            Pune,
            Bhubaneshwar,
            Ahmedgarh,
            Ludhiana,
            Chennai,
            Varanasi,
            Kolkata,
            Denpasar,
            Surabaya,
            Balikpapan,
            Jakarta,
            Manado,
            Jayapura,
            Makassar,
            Bandung,
            Pontianak,
            Manokwari,
            Tehran,
            Baghdad,
            Dublin,
            Douglas,
            Jerusalem,
            Tel_aviv,
            Milan,
            Rome,
            Kingston,
            Kobe,
            Kyoto,
            Nagoya,
            Osaka,
            Sapporo,
            Tokyo,
            Yokohama,
            Amman,
            Almaty,
            Aqtobe,
            Astana,
            Oral,
            Nairobi,
            Kiritimati,
            Pristina,
            Kuwait_city,
            Bishkek,
            Vientiane,
            Riga,
            Beirut,
            Vaduz,
            Vilnius,
            Luxembourg,
            Skopje,
            Antananarivo,
            Kuala_lumpur,
            Male,
            Bamako,
            Valletta,
            Majuro,
            Fort_de_france,
            Nouakchott,
            Port_louis,
            Aguascalientes,
            Mexicali,
            Tijuana,
            Mexico_city,
            Acapulco,
            Guadalajara,
            Cancun,
            Mazatlan,
            Hermosillo,
            Veracruz,
            Palikir,
            Chisinau,
            Monaco,
            Hovd,
            Ulaanbaatar,
            Podgorica,
            Casablanca,
            Rabat,
            Maputo,
            Yangon,
            Windhoek,
            Kathmandu,
            Amsterdam,
            Auckland,
            Wellington,
            Managua,
            Niamey,
            Lagos,
            Alofi,
            Pyongyang,
            Longyearbyen,
            Oslo,
            Tromso,
            Muscat,
            Islamabad,
            Karachi,
            Lahore,
            Ngerulmud,
            Panama,
            Port_moresby,
            Asuncion,
            Lima,
            Manila,
            Warsaw,
            Lisbon,
            Ponta_delgada_azores,
            San_juan,
            Doha,
            Saint_denis,
            Bucharest,
            Belushya_guba,
            Ufa,
            Chelyabinsk,
            Anadyr,
            Pevek,
            Irkutsk,
            Kaliningrad,
            Petropavlovsk_kamchatsky,
            Komsomolsk_on_amur,
            Khatanga,
            Krasnoyarsk,
            Norilsk,
            Magadan,
            Moscow,
            Murmansk,
            Novgorod,
            Novosibirsk,
            Omsk,
            Perm,
            Vladivostok,
            Saint_peterburg,
            Tiksi,
            Verkhoyansk,
            Yakutsk,
            Yuzhno_sakhalinsk,
            Samara,
            Yekaterinburg,
            Kazan,
            Izhevsk,
            Chita,
            Kigali,
            Jamestown,
            Basseterre,
            Castries,
            Kingstown,
            Apia,
            San_marino,
            Sao_tome,
            Makkah,
            Medina,
            Riyadh,
            Dakar,
            Belgrade,
            Victoria,
            Singapore,
            Bratislava,
            Ljubljana,
            Honiara,
            Cape_town,
            Johannesburg,
            Marion_island_prince_edward_islands,
            Pretoria,
            Seoul,
            Barcelona,
            Madrid,
            Palma,
            Colombo,
            Sri_jayawardenapura_kotte,
            Khartoum,
            Paramaribo,
            Stockholm,
            Bern,
            Geneva,
            Zurich,
            Damascus,
            Taipei,
            Dushanbe,
            Dar_es_salaam,
            Dodoma,
            Bangkok,
            Lome,
            Nukualofa,
            Port_of_spain,
            Tunis,
            Ankara,
            Istanbul,
            Ashgabat,
            Funafuti,
            Kampala,
            Dnipro,
            Kyiv,
            Odesa,
            Abu_dhabi,
            Dubai,
            London,
            Belfast,
            Edinburgh,
            Glasgow,
            Cardiff,
            Montevideo,
            Midway,
            Wake_island,
            Montgomery,
            Adak,
            Anchorage,
            Fairbanks,
            Juneau,
            Unalaska,
            Phoenix,
            Little_rock,
            Los_angeles,
            Sacramento,
            San_diego,
            San_francisco,
            Denver,
            Hartford,
            Dover,
            Washington_dc,
            Miami,
            Orlando,
            Pensacola,
            Atlanta,
            Honolulu,
            Boise,
            Chicago,
            Indianapolis,
            Des_moines,
            Topeka,
            Louisville,
            New_orleans,
            Augusta,
            Baltimore,
            Boston,
            Detroit,
            Minneapolis,
            St_paul,
            Jackson,
            Kansas_city,
            St_louis,
            Billings,
            Lincoln,
            Las_vegas,
            Concord,
            Newark,
            Albuquerque,
            New_york,
            Raleigh,
            Bismarck,
            Columbus,
            Oklahoma_city,
            Portland_or,
            Salem,
            Philadelphia,
            Providence,
            Columbia,
            Rapid_city,
            Sioux_falls,
            Knoxville,
            Nashville,
            Austin,
            Dallas,
            Houston,
            Midland,
            Salt_lake_city,
            Montpelier,
            Richmond,
            Seattle,
            Charleston_wv,
            Madison,
            Milwaukee,
            Cheyenne,
            Tashkent,
            Port_vila,
            Vatican_city,
            Caracas,
            Hanoi,
            Ho_chi_minh,
            El_aaiun,
            Lusaka,
            Harare
        }
    }



}
