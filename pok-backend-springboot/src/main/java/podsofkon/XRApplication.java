package podsofkon;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.ser.impl.SimpleFilterProvider;
import oracle.ucp.jdbc.PoolDataSource;
import oracle.ucp.jdbc.PoolDataSourceFactory;
import org.springframework.boot.CommandLineRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.context.properties.EnableConfigurationProperties;
import org.springframework.context.ConfigurableApplicationContext;
import org.springframework.context.annotation.Bean;

import org.springframework.context.annotation.Configuration;
import org.springframework.jms.core.JmsTemplate;

import javax.sql.DataSource;

//@SpringBootApplication(exclude = {
//		DataSourceAutoConfiguration.class,
//		DataSourceTransactionManagerAutoConfiguration.class,
//		HibernateJpaAutoConfiguration.class
//})
@SpringBootApplication
public class XRApplication {

	public static final String REGION = "us-phoenix-1";
	public static final String COMPARTMENT_ID =
			"ocid1.asdf3jja";
	public static final String BUCKET_NAME = "asdf";
	public static final String OBJECTSTORE_NAMESPACENAME = "asdf";

	public static JmsTemplate jmsTemplate;

	public static void main(String[] args) {
//		SpringApplication.run(XRApplication.class, args);
		ConfigurableApplicationContext context = SpringApplication.run(XRApplication.class, args);
//		jmsTemplate = context.getBean(JmsTemplate.class);
	}


	@Configuration
	public class JacksonConfiguration {
		public JacksonConfiguration(ObjectMapper objectMapper) {
			objectMapper.setFilterProvider(new SimpleFilterProvider().setFailOnUnknownId(false));
		}
	}
}
