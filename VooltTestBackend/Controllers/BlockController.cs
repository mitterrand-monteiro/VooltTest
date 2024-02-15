using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace VooltTestBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlocksController : ControllerBase
    {
        public const string BlocksDirectory = "BlocksData/";

        [HttpPost("{key}")]
        public IActionResult CreateBlock(string key)
        {
            var filePath = $"{BlocksDirectory}{key}.json";
            if (!System.IO.File.Exists(filePath))
            {
                var defaultBlocks = GetDefaultBlocks();
                SaveBlockDataToFile(filePath, defaultBlocks);
                return Ok();
            }
            else
            {
                return Conflict("Block with the given key already exists.");
            }
        }

        [HttpGet("{key}")]
        public IActionResult GetBlocks(string key)
        {
            var filePath = $"{BlocksDirectory}{key}.json";
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var blockData = ReadBlockDataFromFile(filePath);
            return Ok(blockData);
        }

        [HttpPut("{key}/{blockType}")]
        public IActionResult UpdateBlock(string key, string blockType, [FromBody] Dictionary<string, object> updatedBlock)
        {
            var filePath = $"{BlocksDirectory}{key}.json";
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var blockData = ReadBlockDataFromFile(filePath);
            if (blockData[blockType] is not Dictionary<string, object> blockToUpdate)
                return NotFound();

            foreach (var item in updatedBlock)
            {
                if (blockToUpdate.ContainsKey(item.Key))
                {
                    blockToUpdate[item.Key] = item.Value;
                }
            }

            blockData[blockType] = blockToUpdate;

            SaveBlockDataToFile(filePath, blockData);
            return Ok();
        }

        [HttpDelete("{key}/{blockType}")]
        public IActionResult RemoveBlock(string key, string blockType)
        {
            var filePath = $"{BlocksDirectory}{key}.json";
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var blockData = ReadBlockDataFromFile(filePath);
            if (blockData.ContainsKey(blockType))
            {
                blockData.Remove(blockType);
                SaveBlockDataToFile(filePath, blockData);
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        private static Dictionary<string, object> GetDefaultBlocks()
        {
            return new Dictionary<string, object>
            {
                { "WebsiteHeader", new Dictionary<string, object>
                    {
                        { "ID", "WebsiteHeader" },
                        { "BlockOrder", 1 },
                        { "BusinessName", "My Business" },
                        { "Logo", new Dictionary<string, object>
                            {
                                { "Text", "Logo" },
                                { "DisplayStatus", true },
                                { "Icon", "logo.png" },
                                { "ButtonEvent", "link" }
                            }
                        },
                        { "NavigationMenu", new List<Dictionary<string, object>>
                            {
                                new Dictionary<string, object>
                                {
                                    { "Text", "Home" },
                                    { "Link", "/" }
                                },
                                new Dictionary<string, object>
                                {
                                    { "Text", "About" },
                                    { "Link", "/about" },
                                    { "SubMenu", new List<Dictionary<string, object>>
                                        {
                                            new Dictionary<string, object>
                                            {
                                                { "Text", "Mission" },
                                                { "Link", "/about/mission" }
                                            },
                                            new Dictionary<string, object>
                                            {
                                                { "Text", "Vision" },
                                                { "Link", "/about/vision" }
                                            }
                                        }
                                    }
                                },
                            }
                        },
                        { "CTAButton", new Dictionary<string, object>
                            {
                                { "Text", "Contact Us" },
                                { "DisplayStatus", true },
                                { "Icon", "phone.png" },
                                { "ButtonEvent", "phone:1234567890" }
                            }
                        }
                    }
                },
                { "WebsiteHeroBlock", new Dictionary<string, object>
                    {
                        { "ID", "WebsiteHeroBlock" },
                        { "BlockOrder", 2 },
                        { "HeadlineText", "Welcome to My Business" },
                        { "DescriptionText", "Your one-stop solution for all your needs." },
                        { "CTAButton", new Dictionary<string, object>
                            {
                                { "Text", "Learn More" },
                                { "DisplayStatus", true },
                                { "Icon", "arrow.png" },
                                { "ButtonEvent", "link:/about" }
                            }
                        },
                        { "HeroImage", "hero.jpg" },
                        { "ImageAlignment", "center" },
                        { "ContentAlignment", "center" }
                    }
                },
                { "ServicesBlock", new Dictionary<string, object>
                    {
                        { "ID", "ServicesBlock" },
                        { "BlockOrder", 3 },
                        { "HeadlineText", "Our Services" },
                        { "ServiceCards", new List<Dictionary<string, object>>
                            {
                                new Dictionary<string, object>
                                {
                                    { "ServiceName", "Service 1" },
                                    { "ServiceDescription", "Description of Service 1" },
                                    { "ServiceImage", "service1.jpg" },
                                    { "ServiceCTAButton", new Dictionary<string, object>
                                        {
                                            { "Text", "Read More" },
                                            { "DisplayStatus", true },
                                            { "Icon", "arrow.png" },
                                            { "ButtonEvent", "link:/services/service1" }
                                        }
                                    }
                                },
                            }
                        }
                    }
                }
            };
        }

        private static void SaveBlockDataToFile(string filePath, Dictionary<string, object> blockData)
        {
            var json = JsonSerializer.Serialize(blockData);
            Directory.CreateDirectory(BlocksDirectory);
            System.IO.File.WriteAllText(filePath, json);
        }

        private static Dictionary<string, object> ReadBlockDataFromFile(string filePath)
        {
            var json = System.IO.File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        }
    }
}
