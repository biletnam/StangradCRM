/*
 * Сделано в SharpDevelop.
 * Пользователь: Дмитрий
 * Дата: 25.08.2016
 * Время: 13:08
 * 
 * Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
 */

namespace StangradCRM.Classes
{
	public enum Role : int {
		NotAuthorized,
		Administrator,
		Director,
		Accountant,
		Technical,
		Manager
	};
}