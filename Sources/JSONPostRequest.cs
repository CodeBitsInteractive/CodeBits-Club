using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

/*============================================*/
/*  Файл:           JSONPostRequest.cs
 *  Назначение:     Обработчик POST-запросов
 *  Разработчик:    CodeBits Interactive
 *  Версия:         1.0
/*============================================*/
namespace CDBitsClub{
    /*============================================*/
    /*  Класс обработки JSON-запросов
    /*============================================*/
    public class JSONPostRequest{
        /* Конструктор класса */
        public JSONPostRequest(){
        }

        /* Отправка запроса на сервер */
        public delegate void OnRequestComplete(string data); // Запрос отправлен
        public delegate void OnRequestError(string code); // Ошибка запроса
        public async Task<bool> sendRequest(string url, FormUrlEncodedContent data, OnRequestComplete complete, OnRequestError error){
            /* Data Form Example */
            /*var formContent = new FormUrlEncodedContent(new[]{
                new KeyValuePair<string, string>("somekey", "1"),
            });*/

            // Создаем клиент и отправляем запрос
            try{
                var myHttpClient = new HttpClient(); // Создать клиент
                var response = await myHttpClient.PostAsync(url, data); // Ждем ответа от сервера
                var json = await response.Content.ReadAsStringAsync(); // Получаем ответ от сервера для конверсии

                // Попытка обработки ответа сервера
                try{ // Успех
                    responceBase resp = JsonConvert.DeserializeObject<responceBase>(json); // Конверсия JSON
                    if (resp.complete){ // Все прошло хорошо
                        complete(json); // Все прошло ОК
                        return true;
                    }else{ // Ошибка
                        error(resp.message); // Выдать ошибку
                        return false;
                    }
                } catch (Exception ex){ // Ошибка
                    error("Failed to convert server responce. Please, try again later."+ json); // Выдать ошибку
                    return false;
                }
            } catch(Exception ex){ // Ошибка отправки запроса
                error("Failed to send responce. Please, check your internet connection and try again."); // Выдать ошибку
                return false;
            }
        }

        /* Get Static Data */
        public delegate void OnGetRequestComplete(string data); // Запрос отправлен
        public delegate void OnGetRequestError(string code); // Ошибка
        public async Task<bool> getSData(string url, OnGetRequestComplete complete, OnGetRequestError error){
            try{
                var myHttpClient = new HttpClient(); // Создать клиент
                var response = await myHttpClient.GetAsync(url); // Ждем ответа от сервера
                var datas = await response.Content.ReadAsStringAsync(); // Получаем ответ от сервера для конверсии

                // Return Datas
                complete(datas); // Все прошло ОК
                return true;
            } catch(Exception ex){
                error("Failed to send responce. Please, check your internet connection and try again."); // Выдать ошибку
                return false;
            }
        }
    }

    #region base_models
    /*============================================*/
    /*  Базовая модель ответа
    /*============================================*/
    public class responceBase{
        public bool complete; // Статус ответа
        public string message; // Сообщение об ошибке (для complete = false)
    }
    #endregion

    #region user_models
    /*============================================*/
    /*  Модель авторизации
    /*============================================*/
    public class authModel{
        public double uid = 0; // Auth UID
        public bool is_auth = false; // Флаг авторизации
        public string login = ""; // Логин
        public string token = ""; // Токен
        public bool is_admin = false; // Права администратора
        public string from = ""; // Тип авторизации
        public double profile_uid = 0; // Profile UID
    }

    /*============================================*/
    /*  Модель профиля
    /*============================================*/
    public class profileModel{
        public double uid = 0; // Profile UID
        public string avatar = ""; // Аватар
        public string nickname = ""; // Никнейм
        public string email = ""; // Email
        public profileData profile_data; // Данные профиля
        public banData ban_data; // Данные блокировки
        public double last_login_day = 0;
    }

    /*============================================*/
    /*  Модель данных профиля
    /*============================================*/
    public class profileData{
        public double ballance = 0; // Балланс
        public string phone = ""; // Телефон
        public string birthday = ""; // День рождения
        public double gold_escape = 0; // Истечение голда
    }

    /*============================================*/
    /*  Модель данных бана
    /*============================================*/
    public class banData{
        public bool banned = false; // Флаг блокировки
        public double ban_escape = 0; // Истечение бана
        public string ban_reason = ""; // Причина бана
    }
    #endregion

    #region news_models
    /*============================================*/
    /*  Модель списка новостей
    /*============================================*/
    public class newsModel{
        public double page = 0; // Текущая страница
        public double total = 0; // Всего страниц
        public Dictionary<string, newsListElement> list; // Список страниц
    }

    /*============================================*/
    /*  Модель элемента списка новостей
    /*============================================*/
    public class newsListElement{
        public double uid = 0; // News UID
        public string image = ""; // News Cover
        public string title = ""; // News Title
        public string slug = ""; // News Slug
        public string desc = ""; // News Desc
        public double time = 0; // News Time
    }
    #endregion

    #region games_model
    /*============================================*/
    /*  Модель списка игр
    /*============================================*/
    public class gamesModel
    {
        public double page = 0; // Текущая страница
        public double total = 0; // Всего страниц
        public Dictionary<string, gamesListElement> list; // Список страниц
    }

    /*============================================*/
    /*  Модель элемента списка игр
    /*============================================*/
    public class gamesListElement
    {
        public double uid = 0; // News UID
        public string image = ""; // News Cover
        public string title = ""; // News Title
        public string slug = ""; // News Slug
        public double time = 0; // News Time
    }
    #endregion

    #region utils_models
    /*============================================*/
    /*  Модель проверки дистрибутива
    /*============================================*/
    public class gameDistModel{
        public bool download = false; // Флаг загрузки
        public string url = ""; // Ссылка для загрузки
        public string version = ""; // Версия игры
    }
    #endregion
}
