using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Runtime.Caching;
using System.Globalization;

namespace Abp.Localization
{
    /// <summary>
    /// Manages host and tenant languages.
    /// </summary>
    public class ApplicationLanguageManager :
        IApplicationLanguageManager,
        IEventHandler<EntityChangedEventData<ApplicationLanguage>>,
        ISingletonDependency
    {
        /// <summary>
        /// Cache name for languages.
        /// </summary>
        public const string CacheName = "AbpZeroLanguages";

        private ITypedCache<int, Dictionary<string, ApplicationLanguage>> LanguageListCache => _cacheManager.GetCache<int, Dictionary<string, ApplicationLanguage>>(CacheName);

        private readonly IRepository<ApplicationLanguage> _languageRepository;
        private readonly ICacheManager _cacheManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationLanguageManager"/> class.
        /// </summary>
        public ApplicationLanguageManager(
            IRepository<ApplicationLanguage> languageRepository,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            ISettingManager settingManager)
        {
            _languageRepository = languageRepository;
            _cacheManager = cacheManager;
            _unitOfWorkManager = unitOfWorkManager;
            _settingManager = settingManager;
        }

        /// <summary>
        /// Gets list of all languages available to given tenant (or null for host)
        /// </summary>
        /// <param name="tenantId">TenantId or null for host</param>
        public virtual async Task<IReadOnlyList<ApplicationLanguage>> GetLanguagesAsync(int? tenantId)
        {
            return (await GetLanguageDictionaryAsync(tenantId)).Values.ToImmutableList();
        }

        public virtual async Task<IReadOnlyList<ApplicationLanguage>> GetActiveLanguagesAsync(int? tenantId)
        {
            return (await GetLanguagesAsync(tenantId)).Where(l => !l.IsDisabled).ToImmutableList();
        }

        /// <summary>
        /// Gets list of all languages available to given tenant (or null for host)
        /// </summary>
        /// <param name="tenantId">TenantId or null for host</param>
        public virtual IReadOnlyList<ApplicationLanguage> GetLanguages(int? tenantId)
        {
            return (GetLanguageDictionary(tenantId)).Values.ToImmutableList();
        }

        public virtual IReadOnlyList<ApplicationLanguage> GetActiveLanguages(int? tenantId)
        {
            return GetLanguages(tenantId).Where(l => !l.IsDisabled).ToImmutableList();
        }

        /// <summary>
        /// Adds a new language.
        /// </summary>
        /// <param name="language">The language.</param>
        [UnitOfWork]
        public virtual async Task AddAsync(ApplicationLanguage language)
        {
            if ((await GetLanguagesAsync(language.TenantId)).Any(l => l.Name == language.Name))
            {
                throw new AbpException("There is already a language with name = " + language.Name);
            }

            using (_unitOfWorkManager.Current.SetTenantId(language.TenantId))
            {
                await _languageRepository.InsertAsync(language);
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Adds a new language.
        /// </summary>
        /// <param name="language">The language.</param>
        [UnitOfWork]
        public virtual void Add(ApplicationLanguage language)
        {
            if ((GetLanguages(language.TenantId)).Any(l => l.Name == language.Name))
            {
                throw new AbpException("There is already a language with name = " + language.Name);
            }

            using (_unitOfWorkManager.Current.SetTenantId(language.TenantId))
            {
                _languageRepository.Insert(language);
                _unitOfWorkManager.Current.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes a language.
        /// </summary>
        /// <param name="tenantId">Tenant Id or null for host.</param>
        /// <param name="languageName">Name of the language.</param>
        [UnitOfWork]
        public virtual async Task RemoveAsync(int? tenantId, string languageName)
        {
            var currentLanguage = (await GetLanguagesAsync(tenantId)).FirstOrDefault(l => l.Name == languageName);
            if (currentLanguage == null)
            {
                return;
            }

            if (currentLanguage.TenantId == null && tenantId != null)
            {
                throw new AbpException("Can not delete a host language from tenant!");
            }

            using (_unitOfWorkManager.Current.SetTenantId(currentLanguage.TenantId))
            {
                await _languageRepository.DeleteAsync(currentLanguage.Id);
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes a language.
        /// </summary>
        /// <param name="tenantId">Tenant Id or null for host.</param>
        /// <param name="languageName">Name of the language.</param>
        [UnitOfWork]
        public virtual void Remove(int? tenantId, string languageName)
        {
            var currentLanguage = (GetLanguages(tenantId)).FirstOrDefault(l => l.Name == languageName);
            if (currentLanguage == null)
            {
                return;
            }

            if (currentLanguage.TenantId == null && tenantId != null)
            {
                throw new AbpException("Can not delete a host language from tenant!");
            }

            using (_unitOfWorkManager.Current.SetTenantId(currentLanguage.TenantId))
            {
                _languageRepository.Delete(currentLanguage.Id);
                _unitOfWorkManager.Current.SaveChanges();
            }
        }

        /// <summary>
        /// Updates a language.
        /// </summary>
        [UnitOfWork]
        public virtual async Task UpdateAsync(int? tenantId, ApplicationLanguage language)
        {
            var existingLanguageWithSameName = (await GetLanguagesAsync(language.TenantId)).FirstOrDefault(l => l.Name == language.Name);
            if (existingLanguageWithSameName != null)
            {
                if (existingLanguageWithSameName.Id != language.Id)
                {
                    throw new AbpException("There is already a language with name = " + language.Name);
                }
            }

            if (language.TenantId == null && tenantId != null)
            {
                throw new AbpException("Can not update a host language from tenant");
            }

            using (_unitOfWorkManager.Current.SetTenantId(language.TenantId))
            {
                await _languageRepository.UpdateAsync(language);
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Updates a language.
        /// </summary>
        [UnitOfWork]
        public virtual void Update(int? tenantId, ApplicationLanguage language)
        {
            var existingLanguageWithSameName = (GetLanguages(language.TenantId)).FirstOrDefault(l => l.Name == language.Name);
            if (existingLanguageWithSameName != null)
            {
                if (existingLanguageWithSameName.Id != language.Id)
                {
                    throw new AbpException("There is already a language with name = " + language.Name);
                }
            }

            if (language.TenantId == null && tenantId != null)
            {
                throw new AbpException("Can not update a host language from tenant");
            }

            using (_unitOfWorkManager.Current.SetTenantId(language.TenantId))
            {
                _languageRepository.Update(language);
                _unitOfWorkManager.Current.SaveChanges();
            }
        }

        /// <summary>
        /// Gets the default language or null for a tenant or the host.
        /// </summary>
        /// <param name="tenantId">Tenant Id of null for host</param>
        public virtual async Task<ApplicationLanguage> GetDefaultLanguageOrNullAsync(int? tenantId)
        {
            var defaultLanguageName = tenantId.HasValue
                ? await _settingManager.GetSettingValueForTenantAsync(LocalizationSettingNames.DefaultLanguage, tenantId.Value)
                : await _settingManager.GetSettingValueForApplicationAsync(LocalizationSettingNames.DefaultLanguage);

            return (await GetLanguagesAsync(tenantId)).FirstOrDefault(l => l.Name == defaultLanguageName);
        }

        /// <summary>
        /// Gets the default language or null for a tenant or the host.
        /// </summary>
        /// <param name="tenantId">Tenant Id of null for host</param>
        public virtual ApplicationLanguage GetDefaultLanguageOrNull(int? tenantId)
        {
            var defaultLanguageName = tenantId.HasValue
                ? _settingManager.GetSettingValueForTenant(LocalizationSettingNames.DefaultLanguage, tenantId.Value)
                : _settingManager.GetSettingValueForApplication(LocalizationSettingNames.DefaultLanguage);

            return (GetLanguages(tenantId)).FirstOrDefault(l => l.Name == defaultLanguageName);
        }

        /// <summary>
        /// Sets the default language for a tenant or the host.
        /// </summary>
        /// <param name="tenantId">Tenant Id of null for host</param>
        /// <param name="languageName">Name of the language.</param>
        public virtual async Task SetDefaultLanguageAsync(int? tenantId, string languageName)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(languageName);
            if (tenantId.HasValue)
            {
                await _settingManager.ChangeSettingForTenantAsync(tenantId.Value, LocalizationSettingNames.DefaultLanguage, cultureInfo.Name);
            }
            else
            {
                await _settingManager.ChangeSettingForApplicationAsync(LocalizationSettingNames.DefaultLanguage, cultureInfo.Name);
            }
        }

        /// <summary>
        /// Sets the default language for a tenant or the host.
        /// </summary>
        /// <param name="tenantId">Tenant Id of null for host</param>
        /// <param name="languageName">Name of the language.</param>
        public virtual void SetDefaultLanguage(int? tenantId, string languageName)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(languageName);
            if (tenantId.HasValue)
            {
                _settingManager.ChangeSettingForTenant(tenantId.Value, LocalizationSettingNames.DefaultLanguage, cultureInfo.Name);
            }
            else
            {
                _settingManager.ChangeSettingForApplication(LocalizationSettingNames.DefaultLanguage, cultureInfo.Name);
            }
        }

        public void HandleEvent(EntityChangedEventData<ApplicationLanguage> eventData)
        {
            LanguageListCache.Remove(eventData.Entity.TenantId ?? 0);

            //Also invalidate the language script cache
            _cacheManager.GetCache("AbpLocalizationScripts").Clear();
        }

        protected virtual async Task<Dictionary<string, ApplicationLanguage>> GetLanguageDictionaryAsync(int? tenantId)
        {
            //Creates a copy of the cached dictionary (to not modify it)
            var languageDictionary = new Dictionary<string, ApplicationLanguage>(await GetLanguageDictionaryFromCacheAsync(null));

            if (tenantId == null)
            {
                return languageDictionary;
            }

            //Override tenant languages
            foreach (var tenantLanguage in await GetLanguageDictionaryFromCacheAsync(tenantId.Value))
            {
                languageDictionary[tenantLanguage.Key] = tenantLanguage.Value;
            }

            return languageDictionary;
        }

        protected virtual Dictionary<string, ApplicationLanguage> GetLanguageDictionary(int? tenantId)
        {
            //Creates a copy of the cached dictionary (to not modify it)
            var languageDictionary = new Dictionary<string, ApplicationLanguage>(GetLanguageDictionaryFromCache(null));

            if (tenantId == null)
            {
                return languageDictionary;
            }

            //Override tenant languages
            foreach (var tenantLanguage in GetLanguageDictionaryFromCache(tenantId.Value))
            {
                languageDictionary[tenantLanguage.Key] = tenantLanguage.Value;
            }

            return languageDictionary;
        }

        private Task<Dictionary<string, ApplicationLanguage>> GetLanguageDictionaryFromCacheAsync(int? tenantId)
        {
            return LanguageListCache.GetAsync(tenantId ?? 0, () => GetLanguagesFromDatabaseAsync(tenantId));
        }

        private Dictionary<string, ApplicationLanguage> GetLanguageDictionaryFromCache(int? tenantId)
        {
            return LanguageListCache.Get(tenantId ?? 0, () => GetLanguagesFromDatabase(tenantId));
        }

        [UnitOfWork]
        protected virtual async Task<Dictionary<string, ApplicationLanguage>> GetLanguagesFromDatabaseAsync(int? tenantId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return (await _languageRepository.GetAllListAsync()).ToDictionary(l => l.Name);
            }
        }

        [UnitOfWork]
        protected virtual Dictionary<string, ApplicationLanguage> GetLanguagesFromDatabase(int? tenantId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return (_languageRepository.GetAllList()).ToDictionary(l => l.Name);
            }
        }
    }
}