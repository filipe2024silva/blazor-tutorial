using f10.pulsar.mes;
using f10.pulsar.mes.data;
using Microsoft.EntityFrameworkCore;

public class UserSettingsService
{
    private readonly MySqlDataContext _db;
    private readonly Utils _utils;

    public UserSettingsService(MySqlDataContext db, Utils utils)
    {
        _db = db;
        _utils = utils;
    }

    public async Task<User> GetUserAsync(int userId)
        => await _db.Users.FirstAsync(x => x.id == userId);

    public async Task SaveUserAsync(User user)
    {
        var entity = await _db.Users.FirstAsync(x => x.id == user.id);
        entity.name = user.name;
        entity.email = user.email;

        await _db.SaveChangesAsync();
        _utils.UserLog("Atualizou os dados do utilizador", "MES");
    }

    public async Task<bool> GetSettingAsync(int userId, string key, bool defaultValue)
    {
        var setting = await _db.Settings
            .FirstOrDefaultAsync(x => x.userid == userId && x.setting == key);

        if (setting != null)
            return Convert.ToBoolean(setting.result);

        _db.Settings.Add(new Settings
        {
            userid = userId,
            setting = key,
            result = defaultValue.ToString()
        });

        await _db.SaveChangesAsync();
        return defaultValue;
    }

    public async Task SaveSettingAsync(int userId, string key, bool value)
    {
        var setting = await _db.Settings
            .FirstAsync(x => x.userid == userId && x.setting == key);

        setting.result = value.ToString();
        await _db.SaveChangesAsync();
    }
}
