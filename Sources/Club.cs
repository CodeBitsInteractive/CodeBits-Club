using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;

/*============================================*/
/*  Файл:           Club.cs
 *  Назначение:     CodeBits Club Class
 *  Разработчик:    CodeBits Interactive
 *  Версия:         1.0
/*============================================*/
namespace CDBitsClub
{
    /*============================================*/
    /*  Класс CodeBits Club
    /*============================================*/
    public class Club{
        // Публичные параметры
        public string server = ""; // Server URL
        public string language = "EN"; // Язык
        public Auth auth; // Класс аутентификатора
        public Profile profile; // Класс профиля
        public News news; // Класс новостей
        public Games games; // Класс игр
        public Utils utils; // Класс утилит
        public JSONPostRequest requests; // Класс выполнения запросов

        // Приватные параметры
        private string LIB_VERSION = "[Beta 0.4]";
        private string LIB_BUILD = "401";

        /* Конструктор Класса */
        public Club(string host = "https://cdbits.net/api", string lang = "EN"){
            // Задаем параметры
            server = host; // Задать сервер
            language = lang; // Задать язык

            // Инициализируем подклассы
            requests = new JSONPostRequest(); // Инициализировать класс
            auth = new Auth(this, server + "/auth/"); // Инициализировать класс
            profile = new Profile(this, server + "/profile/"); // Инициализировать класс
            news = new News(this, server + "/news/"); // Инициализировать класс
            games = new Games(this, server + "/profile/"); // Инициализировать класс
            utils = new Utils(this, server + "/utils/"); // Инициализировать класс
        }

        /* Передать версию клуба */
        public string getClubVersion(){
            return LIB_VERSION; // Передать данные о версии
        }

        /* Передать сборку клуба */
        public string getClubBuild(){
            return LIB_BUILD; // Передать данные о версии
        }

        /* Получить обновления */
        public delegate void OnUpdatesCheckComplete(string version); // Complete
        public delegate void OnUpdatesCheckError(string code); // Error
        public async Task<bool> getUpdates(OnUpdatesCheckComplete done, OnUpdatesCheckError error){
            bool check_updates = await requests.getSData("https://cdbits.net/downloads/launcher/updates.php", ((string ver) => {
                done(ver);
            }), ((string code) => {
                error(code);
            }));

            // Все ок
            return true;
        }
    }

    /*============================================*/
    /*  Класс авторизации CodeBits Club
    /*============================================*/
    public class Auth{
        // Публичные параметры
        public string api_route = "";
        public string access_token = "";
        public authModel credentials;

        // Приватные параметры
        private Club club_instance;

        /* Конструктор Класса */
        public Auth(Club instance, string route = "/auth/"){
            // Задаем параметры
            club_instance = instance; // Передача экземпляра
            api_route = route; // Передача маршрута
            credentials = new authModel(); // Задать модель
        }

        /* Авторизация */
        public delegate void OnSignInComplete(string data);
        public delegate void OnSignInError(string code);
        public async Task<bool> SignIn(string login, string password, OnSignInComplete complete, OnSignInError error){
            // Формируем данные для отправки
            var formContent = new FormUrlEncodedContent(new[]{
                new KeyValuePair<string, string>("login", login),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("lang", club_instance.language)
            });

            // Отправка запроса
            bool auth = await club_instance.requests.sendRequest(api_route+ "login/", formContent, ((string data)=> {
                credentials = JsonConvert.DeserializeObject<authModel>(data); // Конверсия JSON
                access_token = credentials.token; // Установить токен
                complete(access_token);
            }), ((string code) => {
                error(code);
            }));

            // Все ок
            return true;
        }

        /* Получить данные об авторизации по ключу */
        public async Task<bool> SignInByKey(string access_token, OnSignInComplete complete, OnSignInError error){
            // Формируем данные для отправки
            var formContent = new FormUrlEncodedContent(new[]{
                new KeyValuePair<string, string>("access_token", access_token),
                new KeyValuePair<string, string>("lang", club_instance.language)
            });

            // Отправка запроса
            bool auth = await club_instance.requests.sendRequest(api_route + "login/", formContent, ((string data) => {
                credentials = JsonConvert.DeserializeObject<authModel>(data); // Конверсия JSON
                access_token = credentials.token; // Установить токен
                complete(access_token);
            }), ((string code) => {
                error(code);
            }));

            // Все ок
            return true;
        }
    }

    /*============================================*/
    /*  Класс профиля CodeBits Club
    /*============================================*/
    public class Profile{
        // Публичные параметры
        public string api_route = "";
        public profileModel profile; // Данные профиля

        // Приватные параметры
        private Club club_instance;

        /* Конструктор Класса */
        public Profile(Club instance, string route = "/profile/"){
            // Задаем параметры
            club_instance = instance; // Передача экземпляра
            api_route = route; // Передача маршрута

            // Создаем модели данных
            profile = new profileModel(); // Создать модель
            profile.profile_data = new profileData(); // Добавить модель
            profile.ban_data = new banData(); // Добавить модель
        }

        /* Получить данные профиля */
        public delegate void OnProfileLoaded();
        public delegate void OnProfileError(string code);
        public async Task<bool> getProfile(double profile_uid, OnProfileLoaded complete, OnProfileError error){
            // Формируем данные для отправки
            var formContent = new FormUrlEncodedContent(new[]{
                new KeyValuePair<string, string>("access_token", club_instance.auth.access_token),
                new KeyValuePair<string, string>("profile_uid", profile_uid.ToString()),
                new KeyValuePair<string, string>("lang", club_instance.language)
            });

            // Отправка запроса
            bool getprofile = await club_instance.requests.sendRequest(api_route + "get_profile/", formContent, ((string data) => {
                profile = JsonConvert.DeserializeObject<profileModel>(data); // Конверсия JSON
                complete();
            }), ((string code) => {
                error(code);
            }));

            // Все ок
            return true;
        }
    }

    /*============================================*/
    /*  Класс новостей CodeBits Club
    /*============================================*/
    public class News{
        // Публичные параметры
        public string api_route = "";
        public newsModel list;

        // Приватные параметры
        private Club club_instance;

        /* Конструктор Класса */
        public News(Club instance, string route = "/news/"){
            // Задаем параметры
            club_instance = instance; // Передача экземпляра
            api_route = route; // Передача маршрута

            // Создаем модели данных
            list = new newsModel(); // Создать модель новостей
            list.list = new Dictionary<string, newsListElement>(); // Создать список
        }

        /* Получить новости */
        public delegate void OnNewsListLoaded();
        public delegate void OnNewsListError(string code);
        public async Task<bool> getNewsList(double page, string search, OnNewsListLoaded complete, OnNewsListError error){
            // Формируем данные для отправки
            var formContent = new FormUrlEncodedContent(new[]{
                new KeyValuePair<string, string>("access_token", club_instance.auth.access_token),
                new KeyValuePair<string, string>("page", page.ToString()),
                new KeyValuePair<string, string>("search", search),
                new KeyValuePair<string, string>("lang", club_instance.language)
            });

            // Отправка запроса
            bool getList = await club_instance.requests.sendRequest(api_route + "get_list/", formContent, ((string data) => {
                list = JsonConvert.DeserializeObject<newsModel>(data); // Конверсия JSON
                complete();
            }), ((string code) => {
                error(code);
            }));

            // Все ок
            return true;
        }
    }

    /*============================================*/
    /*  Класс игр CodeBits Club
    /*============================================*/
    public class Games
    {
        // Публичные параметры
        public string api_route = "";
        public gamesModel list;

        // Приватные параметры
        private Club club_instance;

        /* Конструктор Класса */
        public Games(Club instance, string route = "/profile/"){
            // Задаем параметры
            club_instance = instance; // Передача экземпляра
            api_route = route; // Передача маршрута

            // Создаем модели данных
            list = new gamesModel(); // Создать модель новостей
            list.list = new Dictionary<string, gamesListElement>(); // Создать список
        }

        /* Получить новости */
        public delegate void OnGamesListLoaded();
        public delegate void OnGamesListError(string code);
        public async Task<bool> getGamesList(double page, string search, OnGamesListLoaded complete, OnGamesListError error){
            // Формируем данные для отправки
            var formContent = new FormUrlEncodedContent(new[]{
                new KeyValuePair<string, string>("access_token", club_instance.auth.access_token),
                new KeyValuePair<string, string>("page", page.ToString()),
                new KeyValuePair<string, string>("lang", club_instance.language)
            });

            // Отправка запроса
            bool getList = await club_instance.requests.sendRequest(api_route + "get_my_games/", formContent, ((string data) => {
                list = JsonConvert.DeserializeObject<gamesModel>(data); // Конверсия JSON
                complete();
            }), ((string code) => {
                error(code);
            }));

            // Все ок
            return true;
        }
    }

    /*============================================*/
    /*  Класс утилит CodeBits Club
    /*============================================*/
    public class Utils{
        // Публичные параметры
        public string api_route = "";
        public gameDistModel game_dist_info;

        // Приватные параметры
        private Club club_instance;

        /* Конструктор Класса */
        public Utils(Club instance, string route = "/utils/"){
            // Задаем параметры
            club_instance = instance; // Передача экземпляра
            api_route = route; // Передача маршрута

            // Создаем модели данных
            game_dist_info = new gameDistModel(); // Создаем модель
        }

        /* Получить данные о дистрибутиве */
        public delegate void OnGameChecked();
        public delegate void OnGameCheckingError(string code);
        public async Task<bool> checkGameDownloads(string slug, string game_version, OnGameChecked complete, OnGameCheckingError error){
            // Формируем данные для отправки
            var formContent = new FormUrlEncodedContent(new[]{
                new KeyValuePair<string, string>("access_token", club_instance.auth.access_token),
                new KeyValuePair<string, string>("game", slug),
                new KeyValuePair<string, string>("version", game_version),
                new KeyValuePair<string, string>("lang", club_instance.language)
            });

            // Отправка запроса
            bool checkGameDownloading = await club_instance.requests.sendRequest(api_route + "get_dist/", formContent, ((string data) => {
                game_dist_info = JsonConvert.DeserializeObject<gameDistModel>(data);
                complete();
            }), ((string code) => {
                error(code);
            }));

            // Все ок
            return true;
        }
    }
}
